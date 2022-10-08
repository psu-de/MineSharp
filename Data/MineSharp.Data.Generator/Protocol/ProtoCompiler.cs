using MineSharp.Data.Generator.Protocol.Datatypes;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
namespace MineSharp.Data.Generator.Protocol
{

    internal class StructureScope
    {

        public StructureScope()
        {
            this.BlockedNames = new List<string>();
        }
        public List<string> BlockedNames { get; set; }

        public bool IsValid(string name)
        {
            name = name.ToLower();
            if (this.BlockedNames.Contains(name)) return false;
            this.BlockedNames.Add(name);
            return true;
        }

        public bool Contains(string name)
        {
            name = name.ToLower();
            return this.BlockedNames.Contains(name);
        }
    }

    internal class ProtoCompiler
    {
        public string? BaseNamespace;

        private ProtoNamespace? CurrentNamespace;
        public string? CurrentProtoNamespace;
        public Dictionary<string, Datatype> DatatypeCache = new Dictionary<string, Datatype>();

        public Dictionary<string, DatatypeGenerator> NativeTypes;

        public List<string> UsedNamespaces = new List<string>();

        public ProtoCompiler()
        {
            this.NativeTypes = new Dictionary<string, DatatypeGenerator>();

            this.LoadNativeTypes();
        }
        public Dictionary<string, Datatype>? DefinedTypes => this.CurrentNamespace?.AllTypes;
        public Dictionary<string, DatatypeGenerator>? UsedNativeTypes => this.CurrentNamespace?.AllNativeTypes;

        private void LoadNativeTypes()
        {
            var conditionalTypes = new Dictionary<string, DatatypeGenerator> {
                {
                    "switch", new SwitchDatatypeGenerator(this)
                }, {
                    "option", new OptionDatatypeGenerator(this)
                }
            };

            var numericTypes = new Dictionary<string, DatatypeGenerator> {
                {
                    "u8", new NumericDatatypeGenerator(this, "U8", "byte")
                }, {
                    "u16", new NumericDatatypeGenerator(this, "U16", "ushort")
                }, {
                    "u32", new NumericDatatypeGenerator(this, "U32", "uint")
                }, {
                    "u64", new NumericDatatypeGenerator(this, "U64", "ulong")
                }, {
                    "i8", new NumericDatatypeGenerator(this, "I8", "sbyte")
                }, {
                    "i16", new NumericDatatypeGenerator(this, "I16", "short")
                }, {
                    "i32", new NumericDatatypeGenerator(this, "I32", "int")
                }, {
                    "i64", new NumericDatatypeGenerator(this, "I64", "long")
                }, {
                    "f32", new NumericDatatypeGenerator(this, "F32", "float")
                }, {
                    "f64", new NumericDatatypeGenerator(this, "F64", "double")
                }, {
                    "varint", new VarIntDatatypeGenerator(this)
                }, {
                    "varlong", new VarIntDatatypeGenerator(this)
                }
            };

            var structuresTypes = new Dictionary<string, DatatypeGenerator> {
                {
                    "array", new ArrayDatatypeGenerator(this)
                }, {
                    "container", new ContainerDatatypeGenerator(this)
                }
            };

            var primitiveTypes = new Dictionary<string, DatatypeGenerator> {
                {
                    "bool", new NumericDatatypeGenerator(this, "Bool", "bool")
                }, {
                    "cstring", new CStringDatatypeGenerator(this)
                }, {
                    "void", new VoidDatatypeGenerator(this)
                }
            };

            var utilTypes = new Dictionary<string, DatatypeGenerator> {
                {
                    "buffer", new BufferDatatypeGenerator(this)
                }, {
                    "bitfield", new BitfieldDatatypeGenerator(this)
                }, {
                    "mapper", new MapperDatatypeGenerator(this)
                }, {
                    "pstring", new PStringDatatypeGenerator(this)
                }
            };

            foreach (var typeDef in conditionalTypes) this.NativeTypes.Add(typeDef.Key, typeDef.Value);

            foreach (var typeDef in numericTypes) this.NativeTypes.Add(typeDef.Key, typeDef.Value);

            foreach (var typeDef in structuresTypes) this.NativeTypes.Add(typeDef.Key, typeDef.Value);

            foreach (var typeDef in primitiveTypes) this.NativeTypes.Add(typeDef.Key, typeDef.Value);

            foreach (var typeDef in utilTypes) this.NativeTypes.Add(typeDef.Key, typeDef.Value);
        }

        public void AddType(string name, DatatypeGenerator generator)
        {
            this.NativeTypes.Add(name, generator);
        }

        public void AddTypes(Dictionary<string, DatatypeGenerator> types)
        {
            foreach (var type in types) this.AddType(type.Key, type.Value);
        }

        public void WriteCode(CodeGenerator codeGenerator, JObject root, string @namespace)
        {
            this.BaseNamespace = @namespace;

            codeGenerator.WriteBlock(this.GetDefaultClasses(@namespace));

            var rootNamespace = new ProtoNamespace(this, null, @namespace, root);

            var stack = new Stack<ProtoNamespace>();
            stack.Push(rootNamespace);


            while (stack.TryPop(out var ns))
            {


                var namespaces = new List<string>();
                var paths = (ns.Token.Path + (ns.Token.Path == "" ? "types" : ".types")).Split('.');
                for (var i = 0; i < paths.Length; i++)
                {
                    namespaces.Add(paths[i]);
                    if (paths[i] == "types") break;
                }
                this.CurrentProtoNamespace = string.Join(".", namespaces);

                this.CurrentNamespace = ns;

                var inner = ns.Parse();
                foreach (var innerNs in inner)
                    stack.Push(innerNs);

                ns.WriteCode(codeGenerator, @namespace);
            }
        }

        internal string GetCSharpName(string name)
        {
            var ti = new CultureInfo("en-US", false).TextInfo;

            if (name.Contains("_"))
            {
                name = name.Replace("_", " ");
            } else
            {
                var clone = name;
                var insertions = 0;
                for (var i = 1; i < name.Length; i++)
                {
                    if (char.IsUpper(name[i]))
                    {
                        clone = clone.Insert(i + insertions++, " ");
                    }
                }
                name = clone;
            }

            name = ti.ToTitleCase(name);

            var rgx = new Regex(@"^\d+");
            var match = rgx.Match(name);
            if (match.Success)
            {
                name = name.Substring(match.Value.Length);
                name += match.Value;
            }

            rgx = new Regex("[^a-zA-Z0-9 -]");
            name = rgx.Replace(name, "");
            name = name.Replace(" ", "");
            return this.Uppercase(name);
        }

        internal string Uppercase(string str) => char.ToUpper(str[0]) + str.Substring(1);

        internal string Lowercase(string str) => char.ToLower(str[0]) + str.Substring(1);

        private string GetDefaultClasses(string @namespace) => $@"namespace {@namespace} {{
    public partial class PacketBuffer {{

        protected MemoryStream _buffer;

        public long Size => _buffer.Length;
        public long ReadableBytes => _buffer.Length - _buffer.Position;
        public long Position => _buffer.Position;


        public PacketBuffer() {{
            _buffer = new MemoryStream();
        }}

        public PacketBuffer(byte[] buffer) {{
            _buffer = new MemoryStream(buffer);
        }}

        public byte[] ToArray() {{
            return _buffer.ToArray();
        }}

        public string HexDump() {{
            return string.Join("" "", ToArray().Select(x => x.ToString(""X2"")));
        }}
        

        public byte[] ReadRaw(int length) {{
            byte[] buffer = new byte[length];
            _buffer.Read(buffer, 0, length);
            return buffer;
        }}

        public void WriteRaw(byte[] data, int offset, int length) {{
            _buffer.Write(data, offset, length);
        }}

        public void WriteRaw(byte[] data) {{
            _buffer.Write(data, 0, data.Length);
        }}
    }}

    public interface IPacketPayload {{
        
    }}

    public interface IPacket {{
		public string Name {{ get; set; }}
    }}

    public interface IPacketFactory {{
        public IPacket ReadPacket(PacketBuffer buffer);
        public void WritePacket(PacketBuffer buffer, IPacketPayload packet);
    }}

    public class VarInt {{

        public bool IsLong => (Value & (long)-4294967296) != 0;

        public long Value;

        public VarInt(long value) {{
            Value = value;
        }}

        public static VarInt Read(PacketBuffer buffer) {{
            long value = 0;
            int shift = 0;

            while (true) {{
                byte b = buffer.ReadU8();
                value |= ((b & (long)0x7f) << shift); // Add the bits to our number, except MSB
                if ((b & 0x80) == 0x00)  // If the MSB is not set, we return the number
                    break;

                shift += 7; // we only have 7 bits, MSB being the return-trigger
                if (shift >= 64) throw new Exception(""varint is too big""); // Make sure our shift don't overflow.
            }}

            return new VarInt(value);
        }}

        public void Write(PacketBuffer buffer) {{
            while ((Value & ~0x7F) != 0x00) {{
                buffer.WriteU8((byte)((Value & 0xFF) | 0x80));
                Value >>= 7;
            }}
            buffer.WriteU8((byte)Value);
        }}

        public static implicit operator int(VarInt value) => (int)value.Value;
        public static implicit operator VarInt(int value) => new VarInt(value);

        public override string ToString() {{
            return this.Value.ToString();
        }}
    }}
}}";
    }
}

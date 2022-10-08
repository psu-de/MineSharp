using MineSharp.Data.Generator.Protocol.Datatypes;
using Newtonsoft.Json.Linq;
namespace MineSharp.Data.Generator.Protocol
{
    internal class ProtoNamespace
    {
        public ProtoNamespace? Parent;

        public Dictionary<string, Datatype> Types;
        public Dictionary<string, DatatypeGenerator> UsedNativeTypes = new Dictionary<string, DatatypeGenerator>();

        public ProtoNamespace(ProtoCompiler compiler, ProtoNamespace? parent, string @namespace, JObject token)
        {
            this.Compiler = compiler;
            this.Parent = parent;
            this.Types = new Dictionary<string, Datatype>();
            this.Namespace = @namespace;
            this.Token = token;
        }

        public ProtoCompiler Compiler { get; set; }
        public Dictionary<string, DatatypeGenerator> AllNativeTypes =>
            new[] {
                    this.UsedNativeTypes, this.Parent?.AllNativeTypes
                }
                .Where(x => x != null)
                .SelectMany(dict => dict!)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        public Dictionary<string, Datatype> AllTypes =>
            new[] {
                    this.Types, this.Parent?.AllTypes
                }
                .Where(x => x != null)
                .SelectMany(dict => dict!)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        public string Namespace { get; set; }
        internal JObject Token { get; set; }

        public ProtoNamespace[] Parse()
        {

            var typesToken = (JObject?)this.Token.GetValue("types");

            if (typesToken != null)
            {

                foreach (var prop in typesToken!.Properties())
                {
                    var name = prop.Name;

                    if (prop.Value.Type == JTokenType.String)
                    {
                        var typeRef = (string)prop.Value!;
                        if (typeRef == "native")
                        {
                            this.UsedNativeTypes.Add(name, this.Compiler.NativeTypes[name]);
                        } else
                        {
                            if (!this.UsedNativeTypes.ContainsKey(typeRef))
                            {
                                this.Types.Add(name, Datatype.Parse(this.Compiler, prop.Value, name, null, null));
                            } else
                            {
                                this.UsedNativeTypes.Add(name, this.UsedNativeTypes[typeRef]);
                            }
                        }
                    } else
                    {
                        var datatype = Datatype.Parse(this.Compiler, prop.Value, name, null, null);
                        this.Types.Add(name, datatype);
                    }
                }
            }

            var innerNamespaces = new List<ProtoNamespace>();
            foreach (var prop in this.Token.Properties().Where(x => x.Name != "types"))
            {
                var ns = this.Compiler.GetCSharpName(prop.Name)
                    .Replace("ToClient", "Clientbound")
                    .Replace("ToServer", "Serverbound");
                var innerNamespace = new ProtoNamespace(this.Compiler, this, this.Namespace + "." + ns, (JObject)prop.Value!);
                innerNamespaces.Add(innerNamespace);
            }
            return innerNamespaces.ToArray();
        }

        private void WritePacketBuffer(CodeGenerator codeGenerator, string baseNamespace)
        {
            codeGenerator.Begin($"namespace {baseNamespace}");
            codeGenerator.Begin("public partial class PacketBuffer");

            codeGenerator.WriteLine("#region Reading");
            codeGenerator.WriteLine();
            foreach (var generator in this.UsedNativeTypes.Select(x => x.Value).DistinctBy(x => x.TypeName))
                generator.WriteClassReader(codeGenerator);
            codeGenerator.WriteLine();
            codeGenerator.WriteLine("#endregion");

            codeGenerator.WriteLine("#region Writing");
            codeGenerator.WriteLine();
            foreach (var generator in this.UsedNativeTypes.Select(x => x.Value).DistinctBy(x => x.TypeName))
                generator.WriteClassWriter(codeGenerator);
            codeGenerator.WriteLine();
            codeGenerator.WriteLine("#endregion");

            codeGenerator.Finish();
            codeGenerator.Finish();
        }

        public void WriteCode(CodeGenerator codeGenerator, string baseNamespace)
        {


            this.WritePacketBuffer(codeGenerator, baseNamespace);

            codeGenerator.Begin($"namespace {this.Namespace}");

            foreach (var type in this.Types)
            {
                type.Value.DoWriteStructure(codeGenerator, null);
            }

            if (this.Types.ContainsKey("packet"))
            {
                var namespaces = this.Namespace.Split(".");
                codeGenerator.Begin($"public class {this.Compiler.GetCSharpName(namespaces[namespaces.Length - 2])}PacketFactory : IPacketFactory");

                codeGenerator.Begin(@"public IPacket ReadPacket(PacketBuffer buffer)");
                codeGenerator.WriteLine($"return {this.Namespace}.Packet.Read(buffer);");
                codeGenerator.Finish();

                var packetType = (ContainerDatatype)this.Types["packet"];
                var @params = (SwitchDatatype)packetType.FieldMap!["params"].Type;

                codeGenerator.Begin(@"public void WritePacket(PacketBuffer buffer, IPacketPayload packet)");
                codeGenerator.Begin("switch (packet)");
                var i = 0;
                foreach (var type in @params.SwitchMap!)
                {
                    codeGenerator.WriteLine(@$"case {type.Value.CSharpType} p_0x{i.ToString("X2")}: new {this.Namespace}.Packet(""{type.Key}"", p_0x{i.ToString("X2")}!).Write(buffer); break;");
                    i++;
                }
                codeGenerator.WriteLine(@$"default: throw new Exception(""{this.Compiler.GetCSharpName(namespaces[namespaces.Length - 2])} cannot write packet of type {{typeof(packet).FullName}}"");");
                codeGenerator.Finish();
                codeGenerator.Finish();
                codeGenerator.Finish();
            }

            codeGenerator.Finish();
        }
    }
}

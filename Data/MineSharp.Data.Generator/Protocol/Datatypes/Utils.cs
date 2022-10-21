using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Generator.Protocol.Datatypes
{

    internal class BufferDatatypeGenerator : DatatypeGenerator<BufferDatatype>
    {
        public BufferDatatypeGenerator(ProtoCompiler compiler) : base(compiler) {}

        public override string TypeName => "buffer";

        public override bool IsGeneric => throw new NotImplementedException();
        public override void WriteClassReader(CodeGenerator codeGenerator)
        {
            codeGenerator.WriteBlock(
                @"public byte[] ReadBuffer(int count) {
    return ReadArray<byte>(count, (PacketBuffer buffer) => buffer.ReadU8());
}");
        }
        public override void WriteClassWriter(CodeGenerator codeGenerator)
        {
            codeGenerator.WriteBlock(
                @"
public void WriteBuffer(byte[] array, Action<PacketBuffer, byte> lengthEncoder) {
    lengthEncoder(this, (byte)array.Length);
    EncodeArray<byte>(array, (buffer, x) => buffer.WriteU8(x));
}

public void WriteBuffer(byte[] array, Action<PacketBuffer, VarInt> lengthEncoder) {
    lengthEncoder(this, array.Length);
    EncodeArray<byte>(array, (buffer, x) => buffer.WriteU8(x));
}"
                );
        }
    }

    internal class BufferDatatype : Datatype
    {
        internal Datatype CountType;
        public BufferDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure)
        {
            this.CountType = Parse(compiler, ((JObject)options!).GetValue("countType")!, name + "Counter", container, outerStructure);
        }
        public override string TypeName => "buffer";

        public override string CSharpType => "byte[]";

        public override string GetReader() => $"((Func<PacketBuffer, byte[]>)((buffer) => buffer.ReadBuffer({this.CountType.GetReader()}(buffer))))";
        protected override void WriteStructure(CodeGenerator codeGenerator, Datatype? parent) {}

        public override string GetWriter() => $"((Action<PacketBuffer, byte[]>)((buffer, value) => buffer.WriteBuffer(value, {this.CountType.GetWriter()})))";
    }

    internal class BitfieldDatatypeGenerator : DatatypeGenerator<BitfieldDatatype>
    {
        public BitfieldDatatypeGenerator(ProtoCompiler compiler) : base(compiler) {}

        public override string TypeName => "bitfield";
        public override bool IsGeneric => false;
        public override void WriteClassReader(CodeGenerator codeGenerator) {}
        public override void WriteClassWriter(CodeGenerator codeGenerator) {}
    }

    internal class BitfieldDatatype : StructureDatatype
    {
        internal int BitCount;

        internal List<BitfieldField>? Fields;
        internal Datatype? ValueDatatype;
        public BitfieldDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {}
        public override string TypeName => "bitfield";

        public override string CSharpType => this.Compiler.GetCSharpName(this.Name) + "Bitfield";

        public override string GetReader()
        {
            if (this.ValueDatatype == null) this.ParseFields();
            return $"((Func<PacketBuffer, {this.CSharpType}>)((buffer) => new {this.CSharpType}({this.ValueDatatype!.GetReader()}(buffer))))";
        }
        public override string GetWriter()
        {
            if (this.ValueDatatype == null) this.ParseFields();
            return $"((Action<PacketBuffer, {this.CSharpType}>)((buffer, value) => {this.ValueDatatype!.GetWriter()}(buffer, value.Value)))";
        }

        protected override void WriteStructure(CodeGenerator codeGenerator, Datatype? parent)
        {

            this.ParseFields();

            codeGenerator.Begin($"public class {this.CSharpType}");

            codeGenerator.WriteLine($"public {this.ValueDatatype!.CSharpType} Value {{ get; set; }}");

            codeGenerator.Begin($"public {this.CSharpType}({this.ValueDatatype!.CSharpType} value)");
            codeGenerator.WriteLine("this.Value = value;");
            codeGenerator.Finish();

            var bitCount = 0;
            foreach (var field in this.Fields!)
            {
                codeGenerator.WriteBlock(
                    $@"public {field.Type.CSharpType} {this.Compiler.GetCSharpName(field.Name)} {{ 
    get {{ 
        return ({field.Type.CSharpType})((({field.Type.CSharpType})Value! >> {this.BitCount - (field.Bits + bitCount)} & ({(1 << field.Bits) - 1})));
    }}
	set {{ 
        var val = value << {this.BitCount - (field.Bits + bitCount)}; 
        var inv = ~val; var x = ({field.Type.CSharpType})this.Value! & ({field.Type.CSharpType})inv; 
        this.Value = ({this.ValueDatatype!.CSharpType})(({field.UnsignedType.CSharpType})x | ({field.UnsignedType.CSharpType})val); 
    }}
}}");
                bitCount += field.Bits;
            }

            codeGenerator.Finish();
        }

        internal Field GetField(string path)
        {
            if (path.Contains("/")) throw new Exception();
            return this.Fields!.First(x => x.Name == path);
        }

        private void ParseFields()
        {
            if (this.Fields != null) return;
            this.Fields = new List<BitfieldField>();
            this.BitCount = 0;

            foreach (JObject obj in (JArray)this.Options!)
            {
                var name = (string)obj.GetValue("name")!;
                var size = (int)obj.GetValue("size")!;
                var signed = (bool)obj.GetValue("signed")!;
                this.Fields.Add(new BitfieldField(name, this.GetFieldDatatype(name, size, signed), size, this.GetFieldDatatype(name, size, false)));
                this.BitCount += size;
            }
            this.ValueDatatype = this.GetFieldDatatype("Value", this.BitCount, false);
        }

        private Datatype GetFieldDatatype(string name, int size, bool signed)
        {
            return size switch {
                (> 0 and <= 8) => (signed ? this.Compiler.NativeTypes["i8"] : this.Compiler.NativeTypes["u8"]).Create(this.Compiler, null, name, this.Container, this),
                (> 8 and <= 16) => (signed ? this.Compiler.NativeTypes["i16"] : this.Compiler.NativeTypes["u16"]).Create(this.Compiler, null, name, this.Container, this),
                (> 16 and <= 32) => (signed ? this.Compiler.NativeTypes["i32"] : this.Compiler.NativeTypes["u32"]).Create(this.Compiler, null, name, this.Container, this),
                (> 32 and <= 64) => (signed ? this.Compiler.NativeTypes["i64"] : this.Compiler.NativeTypes["u64"]).Create(this.Compiler, null, name, this.Container, this),
                _ => throw new Exception()
            };
        }

        internal class BitfieldField : Field
        {

            public int Bits;
            public Datatype UnsignedType;

            public BitfieldField(string name, Datatype type, int bits, Datatype unsignedType) : base(name, type)
            {
                this.Bits = bits;
                this.UnsignedType = unsignedType;
            }
        }
    }

    internal class MapperDatatypeGenerator : DatatypeGenerator<MapperDatatype>
    {
        public MapperDatatypeGenerator(ProtoCompiler compiler) : base(compiler) {}

        public override string TypeName => "mapper";
        public override bool IsGeneric => false;
        public override void WriteClassReader(CodeGenerator codeGenerator) {}
        public override void WriteClassWriter(CodeGenerator codeGenerator) {}
    }

    internal class MapperDatatype : Datatype
    {
        private readonly Dictionary<string, string> Mapping;

        internal Datatype InnerType;

        public MapperDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure)
        {
            this.InnerType = Parse(compiler, ((JObject)options!).GetValue("type")!, name + "Type", container, outerStructure);
            this.Mapping = new Dictionary<string, string>();

            foreach (var prop in ((JObject)((JObject)this.Options!).GetValue("mappings")!).Properties())
            {
                this.Mapping.Add(prop.Name, (string)prop.Value!);
            }
        }
        public override string TypeName => "mapper";

        public override string CSharpType => "string";

        public override string GetReader()
        {
            return $@"((Func<PacketBuffer, string>)((buffer) => {this.InnerType.GetReader()}(buffer){(this.InnerType.CSharpType == "VarInt" ? ".Value" : "")} switch {{ {string.Join(" ", this.Mapping.Select(x => $@"{x.Key} => ""{x.Value}"","))} _ => throw new Exception() }}))";
        }

        protected override void WriteStructure(CodeGenerator codeGenerator, Datatype? parent) {}

        public override string GetWriter()
        {
            return $@"((Action<PacketBuffer, string>)((buffer, value) => {this.InnerType.GetWriter()}(buffer, value switch {{ {string.Join(" ", this.Mapping.Select(x => $@"""{x.Value}"" => {x.Key},"))} _ => throw new Exception($""Value '{{value}}' not supported."") }})))";
        }
    }

    internal class PStringDatatypeGenerator : DatatypeGenerator<PStringDatatype>
    {
        public PStringDatatypeGenerator(ProtoCompiler compiler) : base(compiler)
        {
            compiler.UsedNamespaces.Add("System.Text");
        }

        public override string TypeName => "pstring";
        public override bool IsGeneric => false;

        public override void WriteClassReader(CodeGenerator codeGenerator)
        {
            codeGenerator.WriteBlock(
                @"public string ReadPString(Func<PacketBuffer, VarInt> lengthReader, Encoding? encoding = null) {
    byte[] data = ReadRaw(lengthReader(this));
    return (encoding ?? Encoding.UTF8).GetString(data);
}");
        }
        public override void WriteClassWriter(CodeGenerator codeGenerator)
        {
            codeGenerator.WriteBlock(
                @"public void WritePString(string value, Action<PacketBuffer, VarInt> lengthEncoder, Encoding? encoding = null) {
    byte[] data = (encoding ?? Encoding.UTF8).GetBytes(value);
    lengthEncoder(this, data.Length);
    WriteRaw(data);
}");
        }
    }

    internal class PStringDatatype : Datatype
    {
        public PStringDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {}

        public override string TypeName => "pstring";

        public override string CSharpType => "string";

        public override string GetReader()
        {
            var args = (JObject)this.Options!;
            var countTypeToken = args.GetValue("countType")!;
            var countDatatype = Parse(this.Compiler, countTypeToken, this.Name + "Counter", this.Container, this.OuterStructure);

            return $"((Func<PacketBuffer, {this.CSharpType}>)((buffer) => buffer.ReadPString({countDatatype.GetReader()})))";
        }

        public override string GetWriter()
        {
            var args = (JObject)this.Options!;
            var countTypeToken = args.GetValue("countType")!;
            var countDatatype = Parse(this.Compiler, countTypeToken, this.Name + "Counter", this.Container, this.OuterStructure);

            return $"((Action<PacketBuffer, {this.CSharpType}>)((buffer, value) => buffer.WritePString(value, {countDatatype.GetWriter()})))";
        }

        protected override void WriteStructure(CodeGenerator codeGenerator, Datatype? parent) {}
    }
}

using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Generator.Protocol.Datatypes {

    internal class BufferDatatypeGenerator : DatatypeGenerator<BufferDatatype> {
        public BufferDatatypeGenerator(ProtoCompiler compiler) : base(compiler) {
        }

        public override string TypeName => "buffer";
        public override void WriteClassReader(CodeGenerator codeGenerator) {
            codeGenerator.WriteBlock(
$@"public byte[] ReadBuffer(int count) {{
    return ReadArray<byte>(count, (PacketBuffer buffer) => buffer.ReadU8());
}}");
        }
        public override void WriteClassWriter(CodeGenerator codeGenerator) {
            codeGenerator.WriteBlock(
$@"
public void WriteBuffer(byte[] array, Action<PacketBuffer, byte> lengthEncoder) {{
    lengthEncoder(this, (byte)array.Length);
    EncodeArray<byte>(array, (buffer, x) => buffer.WriteU8(x));
}}

public void WriteBuffer(byte[] array, Action<PacketBuffer, VarInt> lengthEncoder) {{
    lengthEncoder(this, array.Length);
    EncodeArray<byte>(array, (buffer, x) => buffer.WriteU8(x));
}}"
);
        }

        public override bool IsGeneric => throw new NotImplementedException();
    }

    internal class BufferDatatype : Datatype {
        public override string TypeName => "buffer";
        internal Datatype CountType;
        public BufferDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
            this.CountType = Datatype.Parse(compiler, ((JObject)options!).GetValue("countType")!, name + "Counter", container, outerStructure);
        }

        public override string CSharpType => "byte[]";

        public override string GetReader() {
            return $"((Func<PacketBuffer, byte[]>)((buffer) => buffer.ReadBuffer({CountType.GetReader()}(buffer))))";
        }
        protected override void WriteStructure(CodeGenerator codeGenerator, Datatype? parent) { }

        public override string GetWriter() {
            return $"((Action<PacketBuffer, byte[]>)((buffer, value) => buffer.WriteBuffer(value, {CountType.GetWriter()})))";
        }
    }

    internal class BitfieldDatatypeGenerator : DatatypeGenerator<BitfieldDatatype> {
        public BitfieldDatatypeGenerator(ProtoCompiler compiler) : base(compiler) {
        }

        public override string TypeName => "bitfield";
        public override void WriteClassReader(CodeGenerator codeGenerator) { }
        public override void WriteClassWriter(CodeGenerator codeGenerator) { }
        public override bool IsGeneric => false;
    }

    internal class BitfieldDatatype : StructureDatatype {
        public override string TypeName => "bitfield";
        public BitfieldDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
        }

        internal List<BitfieldField>? Fields;
        internal int BitCount = 0;
        internal Datatype? ValueDatatype;

        public override string CSharpType => Compiler.GetCSharpName(Name) + "Bitfield";

        public override string GetReader() {
            if (ValueDatatype == null) ParseFields();
            return $"((Func<PacketBuffer, {CSharpType}>)((buffer) => new {CSharpType}({ValueDatatype!.GetReader()}(buffer))))";
        }
        public override string GetWriter() {
            if (ValueDatatype == null) ParseFields();
            return $"((Action<PacketBuffer, {CSharpType}>)((buffer, value) => {ValueDatatype!.GetWriter()}(buffer, value.Value)))";
        }

        protected override void WriteStructure(CodeGenerator codeGenerator, Datatype? parent) {

            ParseFields();

            codeGenerator.Begin($"public class {CSharpType}");

            codeGenerator.WriteLine($"public {ValueDatatype!.CSharpType} Value {{ get; set; }}");

            codeGenerator.Begin($"public {CSharpType}({ValueDatatype!.CSharpType} value)");
            codeGenerator.WriteLine("this.Value = value;");
            codeGenerator.Finish();

            int bitCount = 0;
            foreach (var field in Fields!) {
                codeGenerator.WriteBlock(
$@"public {field.Type.CSharpType} {Compiler.GetCSharpName(field.Name)} {{ 
    get {{ 
        return ({field.Type.CSharpType})((({field.Type.CSharpType})Value! >> {BitCount - (field.Bits + bitCount)} & ({(1 << field.Bits) - 1})));
    }}
	set {{ 
        var val = value << {BitCount - (field.Bits + bitCount)}; 
        var inv = ~val; var x = ({field.Type.CSharpType})this.Value! & ({field.Type.CSharpType})inv; 
        this.Value = ({ValueDatatype!.CSharpType})(({field.UnsignedType.CSharpType})x | ({field.UnsignedType.CSharpType})val); 
    }}
}}");
                bitCount += field.Bits;
            }

            codeGenerator.Finish();
        }

        internal Field GetField(string path) {
            if (path.Contains("/")) throw new Exception();
            return Fields!.First(x => x.Name == path);
        }

        private void ParseFields() {
            if (Fields != null) return;
            Fields = new List<BitfieldField>();
            BitCount = 0;

            foreach (JObject obj in (JArray)Options!) {
                string name = (string)obj.GetValue("name")!;
                int size = (int)obj.GetValue("size")!;
                bool signed = (bool)obj.GetValue("signed")!;
                Fields.Add(new BitfieldField(name, GetFieldDatatype(name, size, signed), size, GetFieldDatatype(name, size, false)));
                BitCount += size;
            }
            ValueDatatype = GetFieldDatatype("Value", BitCount, false);
        }

        private Datatype GetFieldDatatype(string name, int size, bool signed) {
            return size switch {
                (> 0 and <= 8) => (signed ? Compiler.NativeTypes["i8"] : Compiler.NativeTypes["u8"]).Create(Compiler, null, name, Container, this),
                (> 8 and <= 16) => (signed ? Compiler.NativeTypes["i16"] : Compiler.NativeTypes["u16"]).Create(Compiler, null, name, Container, this),
                (> 16 and <= 32) => (signed ? Compiler.NativeTypes["i32"] : Compiler.NativeTypes["u32"]).Create(Compiler, null, name, Container, this),
                (> 32 and <= 64) => (signed ? Compiler.NativeTypes["i64"] : Compiler.NativeTypes["u64"]).Create(Compiler, null, name, Container, this),
                _ => throw new Exception()
            };
        }

        internal class BitfieldField : Field {

            public int Bits;
            public Datatype UnsignedType;

            public BitfieldField(string name, Datatype type, int bits, Datatype unsignedType) : base(name, type) {
                this.Bits = bits;
                UnsignedType = unsignedType;
            }
        }
    }

    internal class MapperDatatypeGenerator : DatatypeGenerator<MapperDatatype> {
        public MapperDatatypeGenerator(ProtoCompiler compiler) : base(compiler) {
        }

        public override string TypeName => "mapper";
        public override void WriteClassReader(CodeGenerator codeGenerator) { }
        public override void WriteClassWriter(CodeGenerator codeGenerator) { }
        public override bool IsGeneric => false;

    }

    internal class MapperDatatype : Datatype {
        public override string TypeName => "mapper";
        Dictionary<string, string> Mapping;

        internal Datatype InnerType;

        public MapperDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
            InnerType = Datatype.Parse(compiler, ((JObject)options!).GetValue("type")!, name + "Type", container, outerStructure);
            Mapping = new Dictionary<string, string>();

            foreach (var prop in ((JObject)((JObject)Options!).GetValue("mappings")!).Properties()) {
                Mapping.Add(prop.Name, (string)prop.Value!);
            }
        }

        public override string CSharpType => "string";

        public override string GetReader() {
            return $@"((Func<PacketBuffer, string>)((buffer) => {InnerType.GetReader()}(buffer){(InnerType.CSharpType == "VarInt" ? ".Value" : "")} switch {{ {string.Join(" ", Mapping.Select(x => $@"{x.Key} => ""{x.Value}"",")) } _ => throw new Exception() }}))";
        }

        protected override void WriteStructure(CodeGenerator codeGenerator, Datatype? parent) {
        }

        public override string GetWriter() {
            return $@"((Action<PacketBuffer, string>)((buffer, value) => {InnerType.GetWriter()}(buffer, value switch {{ {string.Join(" ", Mapping.Select(x => $@"""{x.Value}"" => {x.Key},")) } _ => throw new Exception($""Value '{{value}}' not supported."") }})))";
        }
    }

    internal class PStringDatatypeGenerator : DatatypeGenerator<PStringDatatype> {
        public PStringDatatypeGenerator(ProtoCompiler compiler) : base(compiler) {
            compiler.UsedNamespaces.Add("System.Text");
        }

        public override string TypeName => "pstring";

        public override void WriteClassReader(CodeGenerator codeGenerator) {
            codeGenerator.WriteBlock(
$@"public string ReadPString(Func<PacketBuffer, VarInt> lengthReader, Encoding? encoding = null) {{
    byte[] data = ReadRaw(lengthReader(this));
    return (encoding ?? Encoding.UTF8).GetString(data);
}}");
        }
        public override void WriteClassWriter(CodeGenerator codeGenerator) {
            codeGenerator.WriteBlock(
$@"public void WritePString(string value, Action<PacketBuffer, VarInt> lengthEncoder, Encoding? encoding = null) {{
    byte[] data = (encoding ?? Encoding.UTF8).GetBytes(value);
    lengthEncoder(this, data.Length);
    WriteRaw(data);
}}");
        }
        public override bool IsGeneric => false;

    }

    internal class PStringDatatype : Datatype {

        public override string TypeName => "pstring";
        public PStringDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
        }

        public override string CSharpType => "string";

        public override string GetReader() {
            var args = (JObject)Options!;
            var countTypeToken = args.GetValue("countType")!;
            var countDatatype = Datatype.Parse(Compiler, countTypeToken, Name + "Counter", this.Container, OuterStructure);

            return $"((Func<PacketBuffer, {CSharpType}>)((buffer) => buffer.ReadPString({countDatatype.GetReader()})))";
        }

        public override string GetWriter() {
            var args = (JObject)Options!;
            var countTypeToken = args.GetValue("countType")!;
            var countDatatype = Datatype.Parse(Compiler, countTypeToken, Name + "Counter", this.Container, OuterStructure);

            return $"((Action<PacketBuffer, {CSharpType}>)((buffer, value) => buffer.WritePString(value, {countDatatype.GetWriter()})))";
        }

        protected override void WriteStructure(CodeGenerator codeGenerator, Datatype? parent) { }
    }
}

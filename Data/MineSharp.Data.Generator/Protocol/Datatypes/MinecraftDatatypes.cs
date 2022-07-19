using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Generator.Protocol.Datatypes {
    internal class UUIDDatatypeGenerator : DatatypeGenerator<UUIDDatatype> {
        public UUIDDatatypeGenerator(ProtoCompiler compiler) : base(compiler) {
            compiler.UsedNamespaces.Add("MineSharp.Core.Types");
        }

        public override string TypeName => "UUID";
        public override void WriteClassReader(CodeGenerator codeGenerator) {
            codeGenerator.WriteBlock(
$@"public UUID ReadUUID() {{
    long l1 = ReadI64();
    long l2 = ReadI64();
    return new UUID(l1, l2);
}}");
        }
        public override void WriteClassWriter(CodeGenerator codeGenerator) {
            codeGenerator.WriteBlock(
$@"public void WriteUUID(UUID value) {{
    WriteI64(value.MostSignificantBits);
    WriteI64(value.LeastSignificantBits);
}}");
        }
        public override bool IsGeneric => false;
    }

    internal class UUIDDatatype : Datatype {
        public UUIDDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
        }

        public override string CSharpType => "UUID";
        public override string TypeName => "UUID";

        public override string GetReader() {
            return $"((Func<PacketBuffer, UUID>)((buffer) => buffer.ReadUUID()))";
        }
        public override string GetWriter() {
            return $"((Action<PacketBuffer, UUID>)((buffer, value) => buffer.WriteUUID(value)))";
        }
        protected override void WriteStructure(CodeGenerator codeGenerator, Datatype? parent) { }
    }

    internal class EntityMetadataLoopGenerator : DatatypeGenerator<EntityMetadataLoopDatatype> {
        public EntityMetadataLoopGenerator(ProtoCompiler compiler) : base(compiler) {
        }

        public override string TypeName => "entityMetadataLoop";
        public override void WriteClassReader(CodeGenerator codeGenerator) {
            codeGenerator.WriteBlock(
    @$"public T[] ReadEntityMetadataLoop<T>(int endVal, Func<PacketBuffer, T> reader) {{
    List<T> data = new List<T>();
    
    while (true) {{
        if (ReadU8() == endVal) {{
            return data.ToArray();
        }} else {{
            _buffer.Position -= 1;
        }}
        data.Add(reader(this));
    }}
}}");
        }

        public override void WriteClassWriter(CodeGenerator codeGenerator) {
            codeGenerator.WriteBlock(
    @$"public void WriteEntityMetadataLoop<T>(T[] values, int endVal, Action<PacketBuffer, T> encoder) {{ 
    for (byte b = 0; b < values.Length; b++) {{
        WriteU8(b);
        encoder(this, values[b]);
    }}
    WriteU8(0xFF);
}}"
    );
        }
        public override bool IsGeneric => false;
    }

    internal class EntityMetadataLoopDatatype : Datatype {

        Datatype InnerType;
        int EndVal;

        public EntityMetadataLoopDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
            this.InnerType = Datatype.Parse(compiler, ((JObject)options!).GetValue("type")!, name + "LoopElement", container, outerStructure);
            this.EndVal = (int)((JObject)options!).GetValue("endVal")!;
        }

        public override string CSharpType => $"{InnerType.CSharpType}[]";
        public override string TypeName => "entityMetadataLoop";

        public override string GetReader() {
            return $"((Func<PacketBuffer, {CSharpType}>)((buffer) => buffer.ReadEntityMetadataLoop({EndVal}, {InnerType.GetReader()})))";
        }

        public override string GetWriter() {
            return $"((Action<PacketBuffer, {CSharpType}>)((buffer, value) => buffer.WriteEntityMetadataLoop(value, {EndVal}, {InnerType.GetWriter()})))";
        }

        protected override void WriteStructure(CodeGenerator codeGenerator, Datatype? parent) {
            InnerType.DoWriteStructure(codeGenerator, parent);
        }
    }

    internal class TopBitSetTerminatedArrayGenerator : DatatypeGenerator<TopBitSetTerminatedArrayDatatype> {
        public TopBitSetTerminatedArrayGenerator(ProtoCompiler compiler) : base(compiler) {
        }

        public override string TypeName => "topBitSetTerminatedArray";
        public override void WriteClassReader(CodeGenerator codeGenerator) {
            codeGenerator.WriteBlock(
@$"public T[] ReadTopBitSetTerminatedArray<T>(Func<PacketBuffer, T> reader) {{
    List<T> data = new List<T>();
    
    while (true) {{
        var next = ReadU8();
        var clone = new MemoryStream();
        clone.WriteByte((byte)(next & 127));
        _buffer.CopyTo(clone);
        _buffer = new MemoryStream(clone.GetBuffer());
        data.Add(reader(this));
        if ((next & 128) == 0) {{
            return data.ToArray();
        }}
    }}
}}");
        }

        public override void WriteClassWriter(CodeGenerator codeGenerator) {
            codeGenerator.WriteBlock(
@$"public void WriteTopBitSetTerminatedArray<T>(T[] values, Action<PacketBuffer, T> encoder) {{ 
    for (int i = 0; i < values.Length; i++) {{
        long pos = this._buffer.Position;
        encoder(this, values[i]);

        if (i == values.Length - 1) {{
            List<byte> buf = new List<byte>();
            var clone = new MemoryStream(_buffer.ToArray());
            
            byte[] b1 = new byte[(int)pos];
            clone.Read(b1, 0, (int)pos);
            buf.AddRange(b1);
            byte b = (byte)clone.ReadByte();
            buf.Add((byte)(b | 128));
            _buffer = new MemoryStream();
            _buffer.Write(buf.ToArray(), 0, buf.Count);
            clone.CopyTo(_buffer);
        }}
    }}
}}"
    );
        }
        public override bool IsGeneric => true;
    }
    internal class TopBitSetTerminatedArrayDatatype : Datatype {
        Datatype InnerType;

        public TopBitSetTerminatedArrayDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
            this.InnerType = Datatype.Parse(compiler, ((JObject)options!).GetValue("type")!, name + "LoopElement", container, outerStructure);
        }
        public override string CSharpType => $"{this.InnerType.CSharpType}[]";

        public override string TypeName => "topBitSetTerminatedArray";

        public override string GetReader() {
            return $"((Func<PacketBuffer, {CSharpType}>)((buffer) => buffer.ReadTopBitSetTerminatedArray({InnerType.GetReader()})))";
        }
        public override string GetWriter() {
            return $"((Action<PacketBuffer, {CSharpType}>)((buffer, value) => buffer.WriteTopBitSetTerminatedArray(value, {InnerType.GetWriter()})))";
        }

        protected override void WriteStructure(CodeGenerator codeGenerator, Datatype? parent) {
            InnerType.DoWriteStructure(codeGenerator, parent);
        }
    }

    internal class RestBufferGenerator : DatatypeGenerator<RestBufferDatatype> {
        public RestBufferGenerator(ProtoCompiler compiler) : base(compiler) {
        }

        public override string TypeName => "restBuffer";

        public override bool IsGeneric => false;

        public override void WriteClassReader(CodeGenerator codeGenerator) {
            codeGenerator.WriteBlock(
@$"public byte[] ReadRestBuffer() {{
    return ReadRaw((int)ReadableBytes);
}}");
        }

        public override void WriteClassWriter(CodeGenerator codeGenerator) {
            codeGenerator.WriteBlock(
@$"public void WriteRestBuffer(byte[] value) {{
    WriteRaw(value);
}}");
        }
    }

    internal class RestBufferDatatype : Datatype {
        public RestBufferDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
        }

        public override string CSharpType => "byte[]";

        public override string TypeName => "restBuffer";

        public override string GetReader() {
            return $"((Func<PacketBuffer, byte[]>)((buffer) => buffer.ReadRestBuffer()))";
        }
        public override string GetWriter() {
            return $"((Action<PacketBuffer, byte[]>)((buffer, value) => buffer.WriteRestBuffer(value)))";
        }

        protected override void WriteStructure(CodeGenerator codeGenerator, Datatype? parent) { }
    }

    internal class NbtDatatypeGenerator : DatatypeGenerator<NbtDatatype> {
        public NbtDatatypeGenerator(ProtoCompiler compiler) : base(compiler) {
            compiler.UsedNamespaces.Add("fNbt");
        }

        public override string TypeName => "nbt";

        public override bool IsGeneric => false;

        public override void WriteClassReader(CodeGenerator codeGenerator) {
            codeGenerator.WriteBlock(
$@"public NbtCompound ReadNbt() {{
    NbtTagType t = (NbtTagType)ReadU8();
    if (t != NbtTagType.Compound) return null;
    _buffer.Position--;

    NbtFile file = new NbtFile() {{BigEndian = true}};

    file.LoadFromStream(_buffer, NbtCompression.None);

    return (NbtCompound)file.RootTag;
}}
");
        }

        public override void WriteClassWriter(CodeGenerator codeGenerator) {
            codeGenerator.WriteBlock(
$@"public void WriteNbt(NbtCompound value) {{
    if (value == null) {{
        WriteU8(0);
        return;
    }}

    NbtFile f = new NbtFile(value) {{ BigEndian = true }};
    f.SaveToStream(_buffer, NbtCompression.None);
}}");
        }
    }

    internal class NbtDatatype : Datatype {
        public NbtDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
        }

        public override string CSharpType => "NbtCompound";

        public override string TypeName => "nbt";

        public override string GetReader() {
            return $"((Func<PacketBuffer, NbtCompound>)((buffer) => buffer.ReadNbt()))";
        }
        public override string GetWriter() {
            return $"((Action<PacketBuffer, NbtCompound>)((buffer, value) => buffer.WriteNbt(value)))";
        }

        protected override void WriteStructure(CodeGenerator codeGenerator, Datatype? parent) {
        }
    }


    internal class OptionalNbtGenerator : DatatypeGenerator<OptionalNbtDatatype> {
        public OptionalNbtGenerator(ProtoCompiler compiler) : base(compiler) {
            compiler.UsedNamespaces.Add("fNbt");
        }

        public override string TypeName => "optionalNbt";

        public override bool IsGeneric => false;

        public override void WriteClassReader(CodeGenerator codeGenerator) {
            codeGenerator.WriteBlock(
$@"public NbtCompound? ReadOptionalNbt() {{
    NbtTagType t = (NbtTagType)ReadU8();
    if (t != NbtTagType.Compound) return null;
    _buffer.Position--;

    NbtFile file = new NbtFile() {{BigEndian = true}};

    file.LoadFromStream(_buffer, NbtCompression.None);

    return (NbtCompound)file.RootTag;
}}
");
        }

        public override void WriteClassWriter(CodeGenerator codeGenerator) {
            codeGenerator.WriteBlock(
$@"public void WriteOptionalNbt(NbtCompound? value) {{
    if (value == null) {{
        WriteU8(0);
        return;
    }}

    NbtFile f = new NbtFile(value) {{ BigEndian = true }};
    f.SaveToStream(_buffer, NbtCompression.None);
}}");
        }
    }

    internal class OptionalNbtDatatype : Datatype {
        public OptionalNbtDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
        }

        public override string CSharpType => "NbtCompound?";

        public override string TypeName => "optionalNbt";

        public override string GetReader() {
            return $"((Func<PacketBuffer, NbtCompound?>)((buffer) => buffer.ReadOptionalNbt()))";
        }

        public override string GetWriter() {
            return $"((Action<PacketBuffer, NbtCompound?>)((buffer, value) => buffer.WriteOptionalNbt(value)))";
        }

        protected override void WriteStructure(CodeGenerator codeGenerator, Datatype? parent) {
        }
    }
}

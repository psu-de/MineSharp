using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Generator.Protocol.Datatypes {
    internal class CStringDatatypeGenerator : DatatypeGenerator {
        public CStringDatatypeGenerator(ProtoCompiler compiler) : base(compiler) {
            compiler.UsedNamespaces.Add("System.Text");
        }

        public override string TypeName => "cstring";
        public override void WriteClassReader(CodeGenerator codeGenerator) {
            codeGenerator.WriteBlock(
$@"public string ReadCString(Encoding? encoding = null) {{
    List<byte> bytes = new List<byte>();
    while ((var b = ReadByte()) != 0) {{
        bytes.Add(b);
    }}
    return (encoding ?? Encoding.UTF8).GetString(bytes.ToArray());
}}");
        }

        public override void WriteClassWriter(CodeGenerator codeGenerator) {
            codeGenerator.WriteBlock(
$@"public void WriteCString(string value, Encoding? encoding = null) {{
    var bytes = (encoding ?? Encoding.UTF8).GetBytes(value);
    WriteRaw(bytes);
    WriteByte(0);
}}");
        }

        public override bool IsGeneric => false;

        public override Datatype Create(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) {
            throw new NotImplementedException();
        }
    }

    internal class VoidDatatypeGenerator : DatatypeGenerator<VoidDatatype> {
        public VoidDatatypeGenerator(ProtoCompiler compiler) : base(compiler) {
        }

        public override string TypeName => "object?";
        public override void WriteClassReader(CodeGenerator codeGenerator) {
            codeGenerator.WriteBlock(
$@"public object? ReadVoid() {{
    return null;
}}");
        }
        public override void WriteClassWriter(CodeGenerator codeGenerator) {
            codeGenerator.WriteBlock($@"public void WriteVoid(object? value) {{ }}");
        }
        public override bool IsGeneric => false;
    }

    internal class VoidDatatype : Datatype {
        public VoidDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
        }

        public override string CSharpType => "object?";

        public override string TypeName => "void";

        public override string GetReader() {
            return $"((Func<PacketBuffer, object?>)((buffer) => buffer.ReadVoid()))";
        }
        public override string GetWriter() {
            return $"((Action<PacketBuffer, object?>)((buffer, value) => buffer.WriteVoid(value)))";
        }

        protected override void WriteStructure(CodeGenerator codeGenerator, Datatype? parent) {
        }
    }
}

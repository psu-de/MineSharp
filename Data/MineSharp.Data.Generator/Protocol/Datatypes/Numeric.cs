using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Generator.Protocol.Datatypes {

    internal class NumericDatatypeGenerator : DatatypeGenerator {

        public override bool IsGeneric => false;
        public override string TypeName => TypeRef;
        public override void WriteClassReader(CodeGenerator codeGenerator) {
            codeGenerator.WriteBlock(
$@"public {CSharpType} Read{this.TypeName}(){{
    byte[] raw = ReadRaw(sizeof({CSharpType}));
    if (BitConverter.IsLittleEndian) Array.Reverse(raw);
    
    {(this.TypeName == "I8" || this.TypeName == "U8" ? $"return {(this.TypeName == "I8" ? "(sbyte)" : "(byte)")}raw[0];" : $"return {GetBitConv()}(raw);")}
    
}}");
        }
        public override void WriteClassWriter(CodeGenerator codeGenerator) {
            if (this.TypeName == "U8" || this.TypeName == "I8") {
                codeGenerator.WriteBlock(
$@"public void Write{this.TypeName}({CSharpType} value) {{
    this._buffer.WriteByte((byte)value);
}}");
            } else {
                codeGenerator.WriteBlock(
$@"public void Write{this.TypeName}({CSharpType} value) {{
    byte[] bytes = BitConverter.GetBytes(value);
    if (BitConverter.IsLittleEndian) Array.Reverse(bytes);

    WriteRaw(bytes);
}}");
            }

        }


        public string CSharpType { get; set; }
        private string TypeRef { get; set; }

        public NumericDatatypeGenerator(ProtoCompiler compiler, string typeName, string csharpType) : base(compiler) {
            this.TypeRef = typeName;
            this.CSharpType = csharpType;
        }

        public override Datatype Create(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? parent, StructureDatatype? outerStructure) {
            return TypeName switch {
                "U8" => new U8Datatype(compiler, options, name, parent, outerStructure),
                "U16" => new U16Datatype(compiler, options, name, parent, outerStructure),
                "U32" => new U32Datatype(compiler, options, name, parent, outerStructure),
                "U64" => new U64Datatype(compiler, options, name, parent, outerStructure),

                "I8" => new I8Datatype(compiler, options, name, parent, outerStructure),
                "I16" => new I16Datatype(compiler, options, name, parent, outerStructure),
                "I32" => new I32Datatype(compiler, options, name, parent, outerStructure),
                "I64" => new I64Datatype(compiler, options, name, parent, outerStructure),

                "F32" => new F32Datatype(compiler, options, name, parent, outerStructure),
                "F64" => new F64Datatype(compiler, options, name, parent, outerStructure),

                "Bool" => new BoolDatatype(compiler, options, name, parent, outerStructure),
                _ => throw  new Exception()
            };
        }

        private string GetBitConv() {
            var t = this.TypeName;

            if (t.StartsWith("U")) {
                t = t.Replace("U", "UInt");
            } else if (t.StartsWith("I")) {
                t = t.Replace("I", "Int");
            } else {
                t = t.Replace("Bool", "Boolean").Replace("F32", "Single").Replace("F64", "Double");
            }
            return $"BitConverter.To{t}";
        }
    }


    internal class VarIntDatatypeGenerator : DatatypeGenerator {
        public VarIntDatatypeGenerator(ProtoCompiler compiler) : base(compiler) {
        }

        public override string TypeName => "varint";
        public override void WriteClassReader(CodeGenerator codeGenerator) {
            codeGenerator.WriteBlock(
@$"public VarInt ReadVarInt() {{
    long value = 0;
    int shift = 0;

    while (true) {{
        byte b = ReadU8();
        value |= ((b & (long)0x7f) << shift); // Add the bits to our number, except MSB
        if ((b & 0x80) == 0x00)  // If the MSB is not set, we return the number
            break;

        shift += 7; // we only have 7 bits, MSB being the return-trigger
        if (shift >= 64) throw new Exception(""varint is too big""); // Make sure our shift don't overflow.
    }}

    return new VarInt(value);
}}");
        }

        public override void WriteClassWriter(CodeGenerator codeGenerator) {
            codeGenerator.WriteBlock(
$@"public void WriteVarInt(VarInt value) {{
    var Value = value.Value;
    while ((Value & ~0x7F) != 0x00) {{
        this.WriteU8((byte)((Value & 0xFF) | 0x80));
        Value >>= 7;
    }}
    this.WriteU8((byte)Value);
}}");
        } 

        public override bool IsGeneric => false;

        public override Datatype Create(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? parent, StructureDatatype? outerStructure) => new VarIntDatatype(compiler, options, name, parent, outerStructure);
    }

    internal class VarIntDatatype : SimpleCSharpTypeDatatype {
        public VarIntDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
        }

        public override string CSharpType => "VarInt";
        public override string TypeName => "VarInt";
    }

    internal class U8Datatype : SimpleCSharpTypeDatatype {
        public U8Datatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
        }

        public override string CSharpType => "byte";
        public override string TypeName => "U8";
    }

    internal class U16Datatype : SimpleCSharpTypeDatatype {
        public U16Datatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
        }

        public override string CSharpType => "ushort";
        public override string TypeName => "U16";
    }

    internal class U32Datatype : SimpleCSharpTypeDatatype {
        public U32Datatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
        }

        public override string CSharpType => "uint";
        public override string TypeName => "U32";
    }

    internal class U64Datatype : SimpleCSharpTypeDatatype {
        public U64Datatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
        }

        public override string CSharpType => "ulong";
        public override string TypeName => "U64";
    }


    internal class I8Datatype : SimpleCSharpTypeDatatype {
        public I8Datatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
        }

        public override string CSharpType => "sbyte";
        public override string TypeName => "I8";
    }

    internal class I16Datatype : SimpleCSharpTypeDatatype {
        public I16Datatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
        }

        public override string CSharpType => "short";
        public override string TypeName => "I16";
    }

    internal class I32Datatype : SimpleCSharpTypeDatatype {
        public I32Datatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
        }

        public override string CSharpType => "int";
        public override string TypeName => "I32";
    }

    internal class I64Datatype : SimpleCSharpTypeDatatype {
        public I64Datatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
        }

        public override string CSharpType => "long";
        public override string TypeName => "I64";
    }

    internal class F32Datatype : SimpleCSharpTypeDatatype {
        public F32Datatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
        }

        public override string CSharpType => "float";
        public override string TypeName => "F32";
    }

    internal class F64Datatype : SimpleCSharpTypeDatatype {
        public F64Datatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
        }

        public override string CSharpType => "double";
        public override string TypeName => "F64";
    }

    internal class BoolDatatype : SimpleCSharpTypeDatatype {
        public BoolDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
        }

        public override string CSharpType => "bool";
        public override string TypeName => "Bool";
    }
}

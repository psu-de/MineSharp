using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Generator.Protocol.Datatypes
{


    internal class ArrayDatatypeGenerator : DatatypeGenerator<ArrayDatatype>
    {
        public ArrayDatatypeGenerator(ProtoCompiler compiler) : base(compiler) {}

        public override string TypeName => "array";
        public override void WriteClassReader(CodeGenerator codeGenerator)
        {
            codeGenerator.WriteBlock(
                $@"public T[] ReadArray<T>(int length, Func<PacketBuffer, T> reader) {{
    T[] array = new T[length];
    for (int i = 0; i < length; i++)
        array[i] = reader(this);
    return array;
}}");
        }

        public override void WriteClassWriter(CodeGenerator codeGenerator)
        {
            codeGenerator.WriteBlock(
                $@"private void EncodeArray<T>(T[] array, Action<PacketBuffer, T> encoder) {{
    for (int i = 0; i < array.Length; i++)
        encoder(this, array[i]);
}}

public void WriteArray<T>(T[] array, Action<PacketBuffer, T> encoder, Action<PacketBuffer, byte> lengthEncoder) {{
    lengthEncoder(this, (byte)array.Length);
    EncodeArray<T>(array, encoder);
}}

public void WriteArray<T>(T[] array, Action<PacketBuffer, T> encoder, Action<PacketBuffer, VarInt> lengthEncoder) {{
    lengthEncoder(this, (VarInt)array.Length);
    EncodeArray<T>(array, encoder);
}}");
        }

        public override bool IsGeneric => false;
    }

    internal class ArrayDatatype : Datatype
    {
        public override string TypeName => "array";

        private Datatype InnerType;
        private Datatype? CountType;
        private string? CountField;

        public ArrayDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure)
        {
            var args = (JObject)options!;
            name = name + "Element";
            this.InnerType = Parse(compiler, args.GetValue("type")!, name, container, outerStructure);

            var countTypeToken = args.GetValue("countType");
            if (countTypeToken != null)
            {
                this.CountType = Parse(compiler, countTypeToken, name + "Counter", container, outerStructure);
            } else
            {
                this.CountField = (string)args.GetValue("count")!;
            }
        }

        public override string CSharpType => this.InnerType.CSharpType + "[]";

        public override string GetReader() => $"((Func<PacketBuffer, {this.CSharpType}>)((buffer) => buffer.ReadArray({(this.CountField != null ? "@" + this.Compiler.Lowercase(this.Compiler.GetCSharpName(this.CountField)) : $"{this.CountType!.GetReader()}(buffer)")}, {this.InnerType.GetReader()})))";
        public override string GetWriter() => $"((Action<PacketBuffer, {this.CSharpType}>)((buffer, value) => buffer.WriteArray(value, {this.InnerType.GetWriter()}, {this.CountType?.GetWriter() ?? this.Container!.GetField(this.CountField!).Type.GetWriter()})))";


        protected override void WriteStructure(CodeGenerator codeGenerator, Datatype? parent)
        {
            this.InnerType.DoWriteStructure(codeGenerator, parent);
        }
    }

    internal class ContainerDatatypeGenerator : DatatypeGenerator<ContainerDatatype>
    {

        public override string TypeName => "container";
        public override void WriteClassReader(CodeGenerator codeGenerator) {}
        public override void WriteClassWriter(CodeGenerator codeGenerator) {}
        public override bool IsGeneric => true;

        public ContainerDatatypeGenerator(ProtoCompiler compiler) : base(compiler) {}
    }

    internal class ContainerDatatype : StructureDatatype
    {

        internal List<Field>? Fields;
        internal Dictionary<string, Field>? FieldMap => this.Fields?.ToDictionary(x => x.Name, x => x);


        public override string TypeName => "container";

        public ContainerDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {}

        public override string CSharpType => this.Compiler.GetCSharpName(this.Name) + (this.Container != null ? "Container" : "");

        public override string GetReader()
        {
            if (this.Fields == null)
            {
                var scope = new StructureScope();
                scope.BlockedNames.Add(this.CSharpType);

                this.ParseFields(scope);
            }
            return $"((Func<PacketBuffer, {this.CSharpType}>)((buffer) => {this.GetCSharpNamespace()}.{this.CSharpType}.Read(buffer {(this.RequiredParentFields.Count == 0 ? "" : $", {string.Join(", ", this.RequiredParentFields.Values.Select(x => $"@{this.Compiler.Lowercase(this.Compiler.GetCSharpName(x.Name))}"))}")})))";
        }

        public override string GetWriter()
        {

            return $"((Action<PacketBuffer, {this.CSharpType}>)((buffer, value) => value.Write(buffer {(this.RequiredParentFields.Count == 0 ? "" : $", {string.Join(", ", this.RequiredParentFields.Values.Select(x => this.OuterStructure is not ContainerDatatype ? $"@{this.Compiler.Lowercase(this.Compiler.GetCSharpName(x.Name))}" : $"{this.Compiler.GetCSharpName(x.Name)}"))}")})))";
        }

        protected override void WriteStructure(CodeGenerator codeGenerator, Datatype? parent)
        {

            var scope = new StructureScope();
            scope.BlockedNames.Add(this.CSharpType);

            this.ParseFields(scope);

            codeGenerator.Begin($"public class {this.CSharpType}{(this.CSharpType == "Packet" ? " : IPacket" : this.CSharpType.StartsWith("Packet") ? " : IPacketPayload" : "")}");

            foreach (var field in this.Fields!)
                field.Type.DoWriteStructure(codeGenerator, this);

            if (this.OuterStructure is not ContainerDatatype)
                foreach (var t in this.RequiredParentFields.ToArray())
                    this.OuterStructure!.RequiredParentFields.TryAdd(t.Key, t.Value);

            foreach (var field in this.Fields!)
                codeGenerator.WriteLine($"public {field.Type.CSharpType} {this.Compiler.GetCSharpName(field.Name)} {{ get; set; }}");

            foreach (var field in this.Fields!.Where(x => x.Name.StartsWith("Anon")))
            {
                switch (field.Type)
                {
                    case ContainerDatatype container:
                        foreach (var cField in container.Fields!)
                            codeGenerator.WriteLine($"public {cField.Type.CSharpType} {this.Compiler.GetCSharpName(cField.Name)} {{ get {{ return {this.Compiler.GetCSharpName(field.Name)}.{this.Compiler.GetCSharpName(cField.Name)}; }} set {{ {this.Compiler.GetCSharpName(field.Name)}.{this.Compiler.GetCSharpName(cField.Name)} = value; }} }}");
                        break;
                    case BitfieldDatatype bitfield:
                        foreach (var bField in bitfield.Fields!)
                            codeGenerator.WriteLine($"public {bField.Type.CSharpType} {this.Compiler.GetCSharpName(bField.Name)} {{ get {{ return {this.Compiler.GetCSharpName(field.Name)}.{this.Compiler.GetCSharpName(bField.Name)}; }} set {{ {this.Compiler.GetCSharpName(field.Name)}.{this.Compiler.GetCSharpName(bField.Name)} = value; }} }}");
                        break;
                }
            }

            codeGenerator.Begin($@"public {this.CSharpType}({string.Join(", ", this.Fields!.Select(x => $"{x.Type.CSharpType} @{this.Compiler.Lowercase(this.Compiler.GetCSharpName(x.Name))}"))})");
            foreach (var field in this.Fields!)
                codeGenerator.WriteLine($"this.{this.Compiler.GetCSharpName(field.Name)} = @{this.Compiler.Lowercase(this.Compiler.GetCSharpName(field.Name))};");
            codeGenerator.Finish();

            codeGenerator.Begin(@$"public void Write(PacketBuffer buffer {(this.RequiredParentFields.Count == 0 ? "" : $", {string.Join(", ", this.RequiredParentFields.Values.Select(x => $"{x.Type.CSharpType} @{this.Compiler.Lowercase(this.Compiler.GetCSharpName(x.Name))}"))}")})");
            foreach (var field in this.Fields!)
            {
                codeGenerator.WriteLine($"{field.Type.GetWriter()}(buffer, this.{this.Compiler.GetCSharpName(field.Name)});");
            }
            codeGenerator.Finish();

            codeGenerator.Begin($@"public static {this.CSharpType} Read(PacketBuffer buffer {(this.RequiredParentFields.Count == 0 ? "" : $", {string.Join(", ", this.RequiredParentFields.Values.Select(x => $"{x.Type.CSharpType} @{this.Compiler.Lowercase(this.Compiler.GetCSharpName(x.Name))}"))}")})");
            foreach (var field in this.Fields!)
            {
                codeGenerator.WriteLine($"{field.Type.CSharpType} @{this.Compiler.Lowercase(this.Compiler.GetCSharpName(field.Name))} = {field.Type.GetReader()}(buffer);");
                if (field.Name.StartsWith("Anon"))
                {
                    switch (field.Type)
                    {
                        case ContainerDatatype container:
                            foreach (var cField in container.Fields!)
                                codeGenerator.WriteLine($"{cField.Type.CSharpType} @{this.Compiler.Lowercase(this.Compiler.GetCSharpName(cField.Name))} = @{this.Compiler.Lowercase(this.Compiler.GetCSharpName(field.Name))}.{this.Compiler.GetCSharpName(cField.Name)};");
                            break;
                        case BitfieldDatatype bitfield:
                            foreach (var bField in bitfield.Fields!)
                                codeGenerator.WriteLine($"{bField.Type.CSharpType} @{this.Compiler.Lowercase(this.Compiler.GetCSharpName(bField.Name))} = @{this.Compiler.Lowercase(this.Compiler.GetCSharpName(field.Name))}.{this.Compiler.GetCSharpName(bField.Name)};");
                            break;
                    }
                }
            }
            codeGenerator.WriteLine($"return new {this.CSharpType}({string.Join(", ", this.Fields!.Select(x => "@" + this.Compiler.Lowercase(this.Compiler.GetCSharpName(x.Name))))});");
            codeGenerator.Finish();


            codeGenerator.Finish();

        }

        private void ParseFields(StructureScope scope)
        {

            if (this.Fields != null) return;
            this.Fields = new List<Field>();

            foreach (JObject obj in (JArray)this.Options!)
            {
                var field = Field.Parse(obj, this);

                if (!scope.IsValid(field.Name))
                {
                    throw new Exception();
                }

                if (field.Type is StructureDatatype)
                {

                    if (!this.IsInParentNamespace((StructureDatatype)field.Type))
                    {
                        if (!scope.IsValid(field.Type.Name))
                        {
                            var c = 1;
                            while (!scope.IsValid(field.Type.Name + c)) c++;
                            field.Type.Name = field.Type.Name + c;
                        }
                    }
                }
                this.Fields.Add(field);
            }
        }

        internal Field GetField(string path)
        {
            var paths = path.Split('/');

            for (var i = 0; i < paths.Length; i++)
            {
                if (paths[i] == "..")
                {
                    return this.Container!.GetField(string.Join('/', paths.Skip(i + 1)));
                }

                if (!this.FieldMap!.TryGetValue(paths[i], out var value))
                {
                    foreach (var anonField in this.Fields!.Where(x => x.Name.StartsWith("Anon")))
                    {
                        if (anonField.Type is StructureDatatype)
                        {
                            if (anonField.Type is ContainerDatatype)
                            {
                                var dtype = ((ContainerDatatype)anonField.Type).GetField(string.Join('/', paths.Skip(i)));
                                if (dtype != null)
                                {
                                    value = dtype;
                                    break;
                                }
                            } else if (anonField.Type is BitfieldDatatype)
                            {
                                return ((BitfieldDatatype)anonField.Type).GetField(string.Join('/', paths.Skip(i + 1)));
                            }
                        }
                    }
                }

                if (value == null) throw new Exception();

                var type = value;
                if (i != paths.Length - 1)
                {
                    if (!(type.Type is StructureDatatype)) throw new Exception();

                    if (type.Type is ContainerDatatype) return ((ContainerDatatype)type.Type).GetField(string.Join('/', paths.Skip(i)));
                    else if (type.Type is BitfieldDatatype)
                    {
                        return ((BitfieldDatatype)type.Type).GetField(string.Join('/', paths.Skip(i + 1)));
                    }
                } else return type;
            }
            throw new Exception();
        }
    }

}

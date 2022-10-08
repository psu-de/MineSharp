using Newtonsoft.Json.Linq;
namespace MineSharp.Data.Generator.Protocol.Datatypes
{
    internal class SwitchDatatypeGenerator : DatatypeGenerator<SwitchDatatype>
    {

        public SwitchDatatypeGenerator(ProtoCompiler compiler) : base(compiler) {}

        public override string TypeName => "switch";
        public override bool IsGeneric => true;
        public override void WriteClassReader(CodeGenerator codeGenerator) {}
        public override void WriteClassWriter(CodeGenerator codeGenerator) {}
    }

    internal class SwitchDatatype : StructureDatatype
    {
        internal Field? CompareToField;
        internal Datatype? DefaultType;
        internal Dictionary<string, Datatype>? SwitchMap;
        public SwitchDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {}

        public override string TypeName => "switch";

        public override string CSharpType => this.Compiler.GetCSharpName(this.Name) + "Switch";

        public override string GetReader()
        {
            this.ParseCompareToDatatype();
            this.ParseSwitchMap();

            var compareTo = (string)((JObject)this.Options!).GetValue("compareTo")!;
            if (compareTo == "$compareTo" && this.ExtraOptions != null)
            {
                compareTo = (string)((JObject)this.ExtraOptions!).GetValue("compareTo")!;
            }
            var paths = compareTo.Split("/");

            if (paths[0] == "..")
            {
                paths = paths.Skip(1).ToArray();
            }


            paths[0] = this.Compiler.Lowercase(this.Compiler.GetCSharpName(paths[0]));
            for (var i = 1; i < paths.Length; i++)
                paths[i] = this.Compiler.GetCSharpName(paths[i]);

            var fieldName = string.Join(".", paths);

            return $"((Func<PacketBuffer, {this.CSharpType}>)((buffer) => {this.CSharpType}.Read(buffer, @{fieldName}{(this.RequiredParentFields!.Count == 0 ? "" : $", {string.Join(", ", this.RequiredParentFields.Values.Select(x => $"@{this.Compiler.Lowercase(this.Compiler.GetCSharpName(x.Name))}"))}")})))";
        }

        public override string GetWriter()
        {
            this.ParseCompareToDatatype();
            this.ParseSwitchMap();

            var compareTo = (string)((JObject)this.Options!).GetValue("compareTo")!;
            if (compareTo == "$compareTo" && this.ExtraOptions != null)
            {
                compareTo = (string)((JObject)this.ExtraOptions!).GetValue("compareTo")!;
            }
            var paths = compareTo.Split("/");

            if (paths[0] == "..")
            {
                paths = paths.Skip(1).ToArray();
                paths[0] = "@" + this.Compiler.Lowercase(this.Compiler.GetCSharpName(paths[0]));
            } else if (this.OuterStructure != null && this.OuterStructure is not ContainerDatatype)
            {
                paths[0] = "@" + this.Compiler.Lowercase(this.Compiler.GetCSharpName(paths[0]));
            } else
            {
                paths[0] = this.Compiler.GetCSharpName(paths[0]);
            }


            for (var i = 1; i < paths.Length; i++)
                paths[i] = this.Compiler.GetCSharpName(paths[i]);

            var fieldName = string.Join(".", paths);

            return $"((Action<PacketBuffer, {this.CSharpType}>)((buffer, value) => value.Write(buffer, {fieldName}{(this.RequiredParentFields!.Count == 0 ? "" : $", {string.Join(", ", this.RequiredParentFields.Values.Select(x => this.Container!.FieldMap!.ContainsKey(x.Name) ? $"{this.Compiler.GetCSharpName(x.Name)}" : $"@{this.Compiler.Lowercase(this.Compiler.GetCSharpName(x.Name))}"))}")})))";
        }

        protected override void WriteStructure(CodeGenerator codeGenerator, Datatype? parent)
        {

            this.ParseCompareToDatatype();
            this.ParseSwitchMap();

            codeGenerator.Begin($"public class {this.CSharpType}");

            foreach (var dtype in this.SwitchMap!.Values)
                dtype.DoWriteStructure(codeGenerator, this.Container);

            codeGenerator.WriteLine("public object? Value { get; set; }");

            codeGenerator.Begin($"public {this.CSharpType}(object? value)");
            codeGenerator.WriteLine("this.Value = value;");
            codeGenerator.Finish();

            codeGenerator.Begin($"public void Write(PacketBuffer buffer, {this.CompareToField!.Type.CSharpType} state{(this.RequiredParentFields.Count == 0 ? "" : $", {string.Join(", ", this.RequiredParentFields.Values.Select(x => $"{x.Type.CSharpType} @{this.Compiler.Lowercase(this.Compiler.GetCSharpName(x.Name))}"))}")})");
            codeGenerator.Begin("switch (state)");
            foreach (var type in this.SwitchMap!)
                codeGenerator.WriteLine($"case {(this.CompareToField!.Type.CSharpType == "string" ? $@"""{type.Key}""" : type.Key)}: {type.Value.GetWriter()}(buffer, ({type.Value.CSharpType})this); break;");
            if (this.DefaultType != null)
                codeGenerator.WriteLine($"default: {this.DefaultType.GetWriter()}(buffer, ({this.DefaultType.CSharpType})Value); break;");
            else
                codeGenerator.WriteLine(@"default: throw new Exception($""Invalid value: '{state}'"");");
            codeGenerator.Finish();
            codeGenerator.Finish();


            codeGenerator.Begin($"public static {this.CSharpType} Read(PacketBuffer buffer, {this.CompareToField!.Type.CSharpType} state{(this.RequiredParentFields.Count == 0 ? "" : $", {string.Join(", ", this.RequiredParentFields.Values.Select(x => $"{x.Type.CSharpType} @{this.Compiler.Lowercase(this.Compiler.GetCSharpName(x.Name))}"))}")})");
            codeGenerator.Begin(@$"object? value = {(this.CompareToField!.Type.CSharpType == "VarInt" ? "state.Value" : "state")} switch");
            foreach (var type in this.SwitchMap!)
                codeGenerator.WriteLine($"{(this.CompareToField!.Type.CSharpType == "string" ? $@"""{type.Key}""" : type.Key)} => {type.Value.GetReader()}(buffer),");
            if (this.DefaultType != null)
                codeGenerator.WriteLine($"_ => {this.DefaultType.GetReader()}(buffer)");
            else if (this.CompareToField!.Type.CSharpType != "bool")
                codeGenerator.WriteLine(@" _ => throw new Exception($""Invalid value: '{state}'"")");

            codeGenerator.Finish(semicolon: true);
            codeGenerator.WriteLine($"return new {this.CSharpType}(value);");
            codeGenerator.Finish();

            var conversions = this.SwitchMap!.Values.Where(x => x.CSharpType != "object?").Select(dtype => $"public static implicit operator {dtype.CSharpType + (dtype.CSharpType.EndsWith("?") ? "" : "?")}({this.CSharpType} value) => ({dtype.CSharpType + (dtype.CSharpType.EndsWith("?") ? "" : "?")})value.Value;").ToList();
            conversions.AddRange(this.SwitchMap!.Values.Where(x => x.CSharpType != "object?").Select(dtype => $"public static implicit operator {this.CSharpType}?({dtype.CSharpType + (dtype.CSharpType.EndsWith("?") ? "" : "?")} value) => new {this.CSharpType}(value);"));
            foreach (var conv in conversions.Distinct())
            {
                codeGenerator.WriteLine(conv);
            }


            codeGenerator.Finish();
        }

        private void ParseCompareToDatatype()
        {

            var defaultToken = ((JObject)this.Options!).GetValue("default");
            if (defaultToken != null)
            {
                this.DefaultType = Parse(this.Compiler, defaultToken, this.Name + "Default", this.Container, this);
            }

            var compareTo = (string)((JObject)this.Options!).GetValue("compareTo")!;

            if (compareTo == "$compareTo")
            {
                if (this.Name == "particleData")
                {
                    this.CompareToField = new Field("particleId", new I32Datatype(this.Compiler, null, "particleDataCompareTo", null, null));
                } else if (this.Name == "entityMetadataItem")
                {
                    this.CompareToField = new Field("type", new VarIntDatatype(this.Compiler, null, "entityMetadataItemCompareTo", null, null));
                } else throw new Exception();
                return;
            }

            var field = this.Container!.GetField(compareTo);
            if (compareTo.StartsWith(".."))
            {
                this.RequiredParentFields.TryAdd(compareTo.Split("/")[1], new Field(compareTo.Split("/")[1], field.Type.OuterStructure != null && field.Type.OuterStructure is not ContainerDatatype ? field.Type.OuterStructure : field.Type));
                this.Container!.RequiredParentFields.TryAdd(compareTo.Split("/")[1], new Field(compareTo.Split("/")[1], field.Type.OuterStructure != null && field.Type.OuterStructure is not ContainerDatatype ? field.Type.OuterStructure : field.Type));
                this.OuterStructure!.RequiredParentFields.TryAdd(compareTo.Split("/")[1], new Field(compareTo.Split("/")[1], field.Type.OuterStructure != null && field.Type.OuterStructure is not ContainerDatatype ? field.Type.OuterStructure : field.Type));
            }

            this.CompareToField = field;
        }

        private void ParseSwitchMap()
        {
            if (this.SwitchMap != null) return;
            this.SwitchMap = new Dictionary<string, Datatype>();
            var mapToken = (JObject)((JObject)this.Options!).GetValue("fields")!;
            foreach (var prop in mapToken.Properties())
            {
                this.SwitchMap!.Add(prop.Name, Parse(this.Compiler, prop.Value, this.CSharpType + "State" + prop.Name, this.Container, this));
            }
        }
    }

    internal class OptionDatatypeGenerator : DatatypeGenerator<OptionDatatype>
    {
        public OptionDatatypeGenerator(ProtoCompiler compiler) : base(compiler) {}

        public override string TypeName => "option";
        public override bool IsGeneric => true;
        public override void WriteClassReader(CodeGenerator codeGenerator)
        {
            codeGenerator.WriteBlock(
                @"public T? ReadOption<T>(Func<PacketBuffer, T> reader) { 
    bool present = this.ReadBool();
    if (!present) return default(T);
    return reader(this);
}");
        }
        public override void WriteClassWriter(CodeGenerator codeGenerator)
        {
            codeGenerator.WriteBlock(
                @"public void WriteOption<T>(T value, Action<PacketBuffer, T> encoder) where T : class {
    if (value == null) {
        WriteBool(false);
        return;
    }
    WriteBool(true);
    encoder(this, value);
}
public void WriteOption<T>(Nullable<T> value, Action<PacketBuffer, T> encoder) where T: struct {
	if (value == null) {
		WriteBool(false);
		return;
	}
	WriteBool(true);
	encoder(this, value.Value);
}");
        }
    }

    internal class OptionDatatype : Datatype
    {

        private readonly Datatype InnerType;

        public OptionDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure)
        {
            this.InnerType = Parse(compiler, options!, name, container, outerStructure);
        }
        public override string TypeName => "options";

        public override string CSharpType => this.InnerType.CSharpType + "?";

        public override string GetReader() => $"((Func<PacketBuffer, {this.CSharpType}>)((buffer) => buffer.ReadOption({this.InnerType.GetReader()})))";
        public override string GetWriter() => $"((Action<PacketBuffer, {this.CSharpType}>)((buffer, value) => buffer.WriteOption(value, {this.InnerType.GetWriter()})))";
        protected override void WriteStructure(CodeGenerator codeGenerator, Datatype? parent)
        {
            this.InnerType.DoWriteStructure(codeGenerator, parent);
        }
    }
}

using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Generator.Protocol.Datatypes {
    internal class SwitchDatatypeGenerator : DatatypeGenerator<SwitchDatatype> {

        public override string TypeName => "switch";
        public override void WriteClassReader(CodeGenerator codeGenerator) { }
        public override void WriteClassWriter(CodeGenerator codeGenerator) { }
        public override bool IsGeneric => true;

        public SwitchDatatypeGenerator(ProtoCompiler compiler) : base(compiler) { }
    }

    internal class SwitchDatatype : StructureDatatype {
        internal Field? CompareToField;
        internal Dictionary<string, Datatype>? SwitchMap;
        internal Datatype? DefaultType;

        public override string TypeName => "switch";
        public SwitchDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
        }

        public override string CSharpType => Compiler.GetCSharpName(Name) + "Switch";

        public override string GetReader() {
            ParseCompareToDatatype();
            ParseSwitchMap();

            string compareTo = (string)((JObject)Options!).GetValue("compareTo")!;
            if (compareTo == "$compareTo" && ExtraOptions != null) {
                compareTo = (string)((JObject)ExtraOptions!).GetValue("compareTo")!;
            }
            var paths = compareTo.Split("/");

            if (paths[0] == "..") {
                paths = paths.Skip(1).ToArray();
            }


            paths[0] = Compiler.Lowercase(Compiler.GetCSharpName(paths[0]));
            for (int i = 1; i < paths.Length; i++)
                paths[i] = Compiler.GetCSharpName(paths[i]);

            string fieldName = string.Join(".", paths);

            return $"((Func<PacketBuffer, {CSharpType}>)((buffer) => {CSharpType}.Read(buffer, @{fieldName}{(RequiredParentFields!.Count == 0 ? "" : $", {string.Join(", ", RequiredParentFields.Values.Select(x => $"@{Compiler.Lowercase(Compiler.GetCSharpName(x.Name))}"))}")})))";
        }

        public override string GetWriter() {
            ParseCompareToDatatype();
            ParseSwitchMap();

            string compareTo = (string)((JObject)Options!).GetValue("compareTo")!;
            if (compareTo == "$compareTo" && ExtraOptions != null) {
                compareTo = (string)((JObject)ExtraOptions!).GetValue("compareTo")!;
            }
            var paths = compareTo.Split("/");

            if (paths[0] == "..") {
                paths = paths.Skip(1).ToArray();
                paths[0] = "@" + Compiler.Lowercase(Compiler.GetCSharpName(paths[0]));
            } else if (OuterStructure != null && OuterStructure is not ContainerDatatype) {
                paths[0] = "@" + Compiler.Lowercase(Compiler.GetCSharpName(paths[0]));
            } else {
                paths[0] = Compiler.GetCSharpName(paths[0]);
            }


            for (int i = 1; i < paths.Length; i++)
                paths[i] = Compiler.GetCSharpName(paths[i]);

            string fieldName = string.Join(".", paths);

            return $"((Action<PacketBuffer, {CSharpType}>)((buffer, value) => value.Write(buffer, {fieldName}{(RequiredParentFields!.Count == 0 ? "" : $", {string.Join(", ", RequiredParentFields.Values.Select(x => (Container!.FieldMap!.ContainsKey(x.Name) ? $"{Compiler.GetCSharpName(x.Name)}" : $"@{Compiler.Lowercase(Compiler.GetCSharpName(x.Name))}")))}")})))";
        }

        protected override void WriteStructure(CodeGenerator codeGenerator, Datatype? parent) {

            ParseCompareToDatatype();
            ParseSwitchMap();

            codeGenerator.Begin($"public class {CSharpType}");

            foreach (var dtype in this.SwitchMap!.Values)
                dtype.DoWriteStructure(codeGenerator, Container);

            codeGenerator.WriteLine("public object? Value { get; set; }");

            codeGenerator.Begin($"public {CSharpType}(object? value)");
            codeGenerator.WriteLine("this.Value = value;");
            codeGenerator.Finish();

            codeGenerator.Begin($"public void Write(PacketBuffer buffer, {CompareToField!.Type.CSharpType} state{(RequiredParentFields.Count == 0 ? "" : $", {string.Join(", ", RequiredParentFields.Values.Select(x => $"{x.Type.CSharpType} @{Compiler.Lowercase(Compiler.GetCSharpName(x.Name))}"))}")})");
            codeGenerator.Begin("switch (state)");
            foreach (var type in SwitchMap!) 
                codeGenerator.WriteLine($"case {(CompareToField!.Type.CSharpType == "string" ? $@"""{type.Key}""" : type.Key)}: {type.Value.GetWriter()}(buffer, ({type.Value.CSharpType})this); break;");
            if (DefaultType != null)
                codeGenerator.WriteLine($"default: {DefaultType.GetWriter()}(buffer, ({DefaultType.CSharpType})Value); break;");
            else
                codeGenerator.WriteLine($@"default: throw new Exception($""Invalid value: '{{state}}'""); break;");
            codeGenerator.Finish();
            codeGenerator.Finish();


            codeGenerator.Begin($"public static {CSharpType} Read(PacketBuffer buffer, {CompareToField!.Type.CSharpType} state{(RequiredParentFields.Count == 0 ? "" : $", {string.Join(", ", RequiredParentFields.Values.Select(x => $"{x.Type.CSharpType} @{Compiler.Lowercase(Compiler.GetCSharpName(x.Name))}"))}")})");
            codeGenerator.Begin(@$"object? value = {(CompareToField!.Type.CSharpType == "VarInt" ? "state.Value" : "state")} switch");
            foreach (var type in SwitchMap!) 
                codeGenerator.WriteLine($"{(CompareToField!.Type.CSharpType == "string" ? $@"""{type.Key}""" : type.Key)} => {type.Value.GetReader()}(buffer),");
            if (DefaultType != null) 
                codeGenerator.WriteLine($"_ => {DefaultType.GetReader()}(buffer)");
            else if (CompareToField!.Type.CSharpType != "bool")
                codeGenerator.WriteLine($@" _ => throw new Exception($""Invalid value: '{{state}}'"")");

            codeGenerator.Finish(semicolon: true);
            codeGenerator.WriteLine($"return new {CSharpType}(value);");
            codeGenerator.Finish();

            List<string> conversions = this.SwitchMap!.Values.Where(x => x.CSharpType != "object?").Select(dtype => $"public static implicit operator {dtype.CSharpType + (dtype.CSharpType.EndsWith("?") ? "" : "?")}({CSharpType} value) => ({dtype.CSharpType + (dtype.CSharpType.EndsWith("?") ? "" : "?")})value.Value;").ToList();
            conversions.AddRange(this.SwitchMap!.Values.Where(x => x.CSharpType != "object?").Select(dtype => $"public static implicit operator {CSharpType}?({dtype.CSharpType + (dtype.CSharpType.EndsWith("?") ? "" : "?")} value) => new {CSharpType}(value);"));
            foreach (var conv in conversions.Distinct()) {
                codeGenerator.WriteLine(conv);
            }


            codeGenerator.Finish();
        }

        private void ParseCompareToDatatype() {

            var defaultToken = ((JObject)Options!).GetValue("default");
            if (defaultToken != null) {
                DefaultType = Datatype.Parse(Compiler, defaultToken, Name + "Default", Container, this);
            }

            string compareTo = (string)((JObject)Options!).GetValue("compareTo")!;

            if (compareTo == "$compareTo") {
                if (Name == "particleData") {
                    this.CompareToField = new Field("particleId", new Datatypes.I32Datatype(Compiler, null, "particleDataCompareTo", null, null));
                } else if (Name == "entityMetadataItem") {
                    this.CompareToField = new Field("type", new Datatypes.VarIntDatatype(Compiler, null, "entityMetadataItemCompareTo", null, null));
                } else throw new Exception();
                return;
            }

            var field = Container!.GetField(compareTo);
            if (compareTo.StartsWith("..")) {
                this.RequiredParentFields.TryAdd(compareTo.Split("/")[1], new Field(compareTo.Split("/")[1], (field.Type.OuterStructure != null && field.Type.OuterStructure is not ContainerDatatype) ? field.Type.OuterStructure : field.Type));
                this.Container!.RequiredParentFields.TryAdd(compareTo.Split("/")[1], new Field(compareTo.Split("/")[1], (field.Type.OuterStructure != null && field.Type.OuterStructure is not ContainerDatatype) ? field.Type.OuterStructure : field.Type));
                this.OuterStructure!.RequiredParentFields.TryAdd(compareTo.Split("/")[1], new Field(compareTo.Split("/")[1], (field.Type.OuterStructure != null && field.Type.OuterStructure is not ContainerDatatype) ? field.Type.OuterStructure : field.Type));
            }

            this.CompareToField = field;
        }

        private void ParseSwitchMap() {
            if (SwitchMap != null) return;
            this.SwitchMap = new Dictionary<string, Datatype>();
            JObject mapToken = (JObject)((JObject)Options!).GetValue("fields")!;
            foreach (var prop in mapToken.Properties()) {
                this.SwitchMap!.Add(prop.Name, Datatype.Parse(Compiler, prop.Value, this.CSharpType + "State" + prop.Name, Container, this));
            }
        }
    }

    internal class OptionDatatypeGenerator : DatatypeGenerator<OptionDatatype> {
        public OptionDatatypeGenerator(ProtoCompiler compiler) : base(compiler) {
        }

        public override string TypeName => "option";
        public override void WriteClassReader(CodeGenerator codeGenerator) {
            codeGenerator.WriteBlock(
@"public T? ReadOption<T>(Func<PacketBuffer, T> reader) { 
    bool present = this.ReadBool();
    if (!present) return default(T);
    return reader(this);
}");
        }
        public override void WriteClassWriter(CodeGenerator codeGenerator) {
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
        public override bool IsGeneric => true;
    }

    internal class OptionDatatype : Datatype {

        Datatype InnerType;
        public override string TypeName => "options";

        public OptionDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
            this.InnerType = Datatype.Parse(compiler, options!, name, container, outerStructure);
        }

        public override string CSharpType => InnerType.CSharpType + "?";

        public override string GetReader() {
            return $"((Func<PacketBuffer, {CSharpType}>)((buffer) => buffer.ReadOption({InnerType.GetReader()})))";
        }
        public override string GetWriter() {
            return $"((Action<PacketBuffer, {CSharpType}>)((buffer, value) => buffer.WriteOption(value, {InnerType.GetWriter()})))";
        }
        protected override void WriteStructure(CodeGenerator codeGenerator, Datatype? parent) {
            InnerType.DoWriteStructure(codeGenerator, parent);
        }
    }
}

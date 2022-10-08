using Newtonsoft.Json.Linq;
namespace MineSharp.Data.Generator.Protocol.Datatypes
{

    internal class DatatypeNotFoundException : Exception {}

    internal abstract class Datatype
    {


        private bool IsStructureWritten;

        public Datatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure)
        {
            this.Compiler = compiler;
            this.Options = options;
            this.Name = name;
            this.Container = container;
            this.OuterStructure = outerStructure;

            if (options != null)
            {
                this.Namespace = "";
                var namespaces = new List<string>();
                var paths = options.Path.Split('.');
                for (var i = 0; i < paths.Length; i++)
                {
                    namespaces.Add(paths[i]);
                    if (paths[i] == "types") break;
                }
                this.Namespace = string.Join(".", namespaces);
                if (this.Namespace == "") this.Namespace = "types";
            }
        }

        public ProtoCompiler Compiler { get; set; }
        public JToken? Options { get; set; }
        public JToken? ExtraOptions { get; set; }
        public string Name { get; set; }
        public abstract string CSharpType { get; }
        public abstract string TypeName { get; }
        public ContainerDatatype? Container { get; set; }
        public StructureDatatype? OuterStructure { get; set; }
        public string? Namespace { get; set; }

        public string GetCSharpNamespace()
        {

            if (this.OuterStructure != null)
            {
                return this.OuterStructure.GetCSharpNamespace() + "." + this.OuterStructure.CSharpType;
            }

            var namespaces = this.Namespace!.Split(".");
            var thisNs = string.Join(".", namespaces.Take(namespaces.ToList().IndexOf("types")).Select(x => this.Compiler.GetCSharpName(x)))
                .Replace("ToClient", "Clientbound")
                .Replace("ToServer", "Serverbound");

            return $"{this.Compiler.BaseNamespace}{(thisNs != "" ? "." + thisNs : "")}";
        }

        public bool IsInParentNamespace(Datatype dtype)
        {
            if (this.Namespace == null || dtype.Namespace == null) throw new Exception();
            var thisPaths = this.Namespace.Split('.');
            var otherPaths = dtype.Namespace.Split('.');

            if (otherPaths.Length > thisPaths.Length) return false;

            for (var i = 0; i < otherPaths.Length; i++)
            {
                if (otherPaths[i] == "types") return true;
                if (otherPaths[i] != thisPaths[i]) return false;
            }
            throw new Exception();
        }

        public abstract string GetReader();
        public abstract string GetWriter();

        public static Datatype Parse(ProtoCompiler compiler, JToken token, string name, ContainerDatatype? parent, StructureDatatype? outerStructure)
        {

            if (compiler.DatatypeCache.ContainsKey(token.Path)) return compiler.DatatypeCache[token.Path];

            string typeRef;
            JToken? options = null;

            if (token.Type == JTokenType.String)
            {
                typeRef = (string)token!;
            } else
            {
                typeRef = (string)((JArray)token)[0]!;
                options = token[1]!;
            }

            var newDtype = Create(compiler, typeRef, options, name, parent, outerStructure);
            compiler.DatatypeCache.Add(token.Path, newDtype);
            return newDtype;
        }

        public static Datatype Create(ProtoCompiler compiler, string typeRef, JToken? options, string name, ContainerDatatype? parent, StructureDatatype? outerStructure)
        {


            if (compiler.UsedNativeTypes!.ContainsKey(typeRef))
            {
                var generator = compiler.UsedNativeTypes[typeRef];
                var datatype = generator.Create(compiler, options, name, parent, outerStructure);
                return datatype;
            }

            if (!compiler.DefinedTypes!.TryGetValue(typeRef, out var dtype))
            {
                if (compiler.NativeTypes!.TryGetValue(typeRef, out var dtypeGen))
                {
                    return dtypeGen!.Create(compiler, options, name, parent, outerStructure); // For some reason, the native type 'mapper' is not defined in the minecraft-data protocol json as a native type, but is still used
                }
                throw new DatatypeNotFoundException();
            }
            //if (options != null) {
            //    options = MergeOptions((JObject)dtype.Options!, (JObject)options);
            //    return Create(compiler, dtype.TypeName, options, name, parent, outerStructure);
            //}
            if (options != null)
            {
                dtype.ExtraOptions = options;
            }
            return dtype;

        }

        private static JObject MergeOptions(JObject baseObj, JObject extended)
        {
            var clone = (JObject)baseObj.DeepClone();
            foreach (var prop in extended.Properties())
            {
                clone[prop.Name] = prop.Value;
            }
            return clone;
        }

        public void DoWriteStructure(CodeGenerator codeGenerator, Datatype? writer)
        {
            if (this.IsStructureWritten) return;
            // only write structure if in correct namespace

            if (this.Container != writer) return;

            if (this.Compiler.CurrentProtoNamespace == this.Namespace)
            {
                this.IsStructureWritten = true;
                this.WriteStructure(codeGenerator, writer);
            }
        }

        protected abstract void WriteStructure(CodeGenerator codeGenerator, Datatype? writer);
    }

    internal abstract class SimpleCSharpTypeDatatype : Datatype
    {

        public SimpleCSharpTypeDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {}

        public override string GetReader() => $"((Func<PacketBuffer, {this.CSharpType}>)((buffer) => buffer.Read{this.TypeName}()))";
        public override string GetWriter() => $"((Action<PacketBuffer, {this.CSharpType}>)((buffer, value) => buffer.Write{this.TypeName}(value)))";

        protected override void WriteStructure(CodeGenerator codeGenerator, Datatype? parent) {}
    }


    internal abstract class DatatypeGenerator
    {

        public ProtoCompiler Compiler;

        public DatatypeGenerator(ProtoCompiler compiler)
        {
            this.Compiler = compiler;
        }

        public abstract string TypeName { get; }
        public abstract bool IsGeneric { get; }

        public abstract Datatype Create(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? parent, StructureDatatype? outerStructure);

        public abstract void WriteClassReader(CodeGenerator codeGenerator);
        public abstract void WriteClassWriter(CodeGenerator codeGenerator);
    }

    internal abstract class DatatypeGenerator<T> : DatatypeGenerator where T : Datatype
    {

        public DatatypeGenerator(ProtoCompiler compiler) : base(compiler) {}

        public override Datatype Create(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? parent, StructureDatatype? outerStructure)
        {
            var type = typeof(T);
            return (Datatype)Activator.CreateInstance(type, compiler, options, name, parent, outerStructure)!;
        }
    }

    internal abstract class StructureDatatype : Datatype
    {

        internal Dictionary<string, Field> RequiredParentFields = new Dictionary<string, Field>();
        protected StructureDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure)
        {
            if (options == null) throw new Exception();
        }

        internal class Field
        {
            public string Name;
            public Datatype Type;

            public Field(string name, Datatype type)
            {
                this.Name = name;
                this.Type = type;
            }

            public static Field Parse(JObject obj, ContainerDatatype current)
            {


                var name = (string?)obj.GetValue("name");
                if (name == null)
                {
                    name = "Anon";
                }

                var type = Datatype.Parse(current.Compiler, obj.GetValue("type")!, name, current, current);

                return new Field(name, type);
            }
        }
    }
}

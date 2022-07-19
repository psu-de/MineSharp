using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Generator.Protocol.Datatypes {

    internal class DatatypeNotFoundException : Exception { }

    internal abstract class Datatype {

        public ProtoCompiler Compiler { get; set; }
        public JToken? Options { get; set; }
        public JToken? ExtraOptions { get; set; }
        public string Name { get; set; }
        public abstract string CSharpType { get; }
        public abstract string TypeName { get; }
        public ContainerDatatype? Container { get; set; }
        public StructureDatatype? OuterStructure { get; set; }
        public string? Namespace { get; set; }


        private bool IsStructureWritten = false;

        public Datatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) {
            this.Compiler = compiler;
            this.Options = options;
            this.Name = name;
            Container = container;
            OuterStructure = outerStructure;

            if (options != null) {
                Namespace = "";
                List<string> namespaces = new List<string>();
                string[] paths = options.Path.Split('.');
                for (int i = 0; i < paths.Length; i++) {
                    namespaces.Add(paths[i]);
                    if (paths[i] == "types") break;
                }
                Namespace = string.Join(".", namespaces);
                if (Namespace == "")
                    Namespace = "types";
            }
        }

        public string GetCSharpNamespace() {

            if (OuterStructure != null) {
                return OuterStructure.GetCSharpNamespace() + "." + OuterStructure.CSharpType;
            }

            var namespaces = Namespace!.Split(".");
            var thisNs = string.Join(".", namespaces.Take(namespaces.ToList().IndexOf("types")).Select(x => Compiler.GetCSharpName(x)))
                .Replace("ToClient", "Clientbound")
                .Replace("ToServer", "Serverbound");

            return $"{Compiler.BaseNamespace}{(thisNs != "" ? "." + thisNs : "")}";
        }

        public bool IsInParentNamespace(Datatype dtype) {
            if (Namespace == null || dtype.Namespace == null) throw new Exception();
            var thisPaths = Namespace.Split('.');
            var otherPaths = dtype.Namespace.Split('.');

            if (otherPaths.Length > thisPaths.Length) return false;

            for (int i = 0; i < otherPaths.Length; i++) {
                if (otherPaths[i] == "types") return true;
                if (otherPaths[i] != thisPaths[i]) return false;
            }
            throw new Exception();
        }

        public abstract string GetReader();
        public abstract string GetWriter();

        public static Datatype Parse(ProtoCompiler compiler, JToken token, string name, ContainerDatatype? parent, StructureDatatype? outerStructure) {

            if (compiler.DatatypeCache.ContainsKey(token.Path)) return compiler.DatatypeCache[token.Path];

            string typeRef;
            JToken? options = null;

            if (token.Type == JTokenType.String) {
                typeRef = (string)token!;
            } else {
                typeRef = (string)((JArray)token)[0]!;
                options = token[1]!;
            }

            var newDtype = Create(compiler, typeRef, options, name, parent, outerStructure);
            compiler.DatatypeCache.Add(token.Path, newDtype);
            return newDtype;
        }

        public static Datatype Create(ProtoCompiler compiler, string typeRef, JToken? options, string name, ContainerDatatype? parent, StructureDatatype? outerStructure) {


            if (compiler.UsedNativeTypes!.ContainsKey(typeRef)) {
                var generator = compiler.UsedNativeTypes[typeRef];
                var datatype = generator.Create(compiler, options, name, parent, outerStructure);
                return datatype;
            }

            if (!compiler.DefinedTypes!.TryGetValue(typeRef, out var dtype)) {
                if (compiler.NativeTypes!.TryGetValue(typeRef, out var dtypeGen)) {
                    return dtypeGen!.Create(compiler, options, name, parent, outerStructure); // For some reason, the native type 'mapper' is not defined in the minecraft-data protocol json as a native type, but is still used
                } else {
                    throw new DatatypeNotFoundException();
                }
            } else {
                //if (options != null) {
                //    options = MergeOptions((JObject)dtype.Options!, (JObject)options);
                //    return Create(compiler, dtype.TypeName, options, name, parent, outerStructure);
                //}
                if (options != null) {
                    dtype.ExtraOptions = options;
                }
                return dtype;
            }

        }

        private static JObject MergeOptions(JObject baseObj, JObject extended) {
            var clone = (JObject)baseObj.DeepClone();
            foreach (var prop in extended.Properties()) {
                clone[prop.Name] = prop.Value;
            }
            return clone;
        }

        public void DoWriteStructure(CodeGenerator codeGenerator, Datatype? writer) { 
            if (IsStructureWritten) return;
            // only write structure if in correct namespace

            if (Container != writer) return;

            if (Compiler.CurrentProtoNamespace == this.Namespace) {
                IsStructureWritten = true;
                WriteStructure(codeGenerator, writer);
                return;
            }
        }

        protected abstract void WriteStructure(CodeGenerator codeGenerator, Datatype? writer);
    }

    internal abstract class SimpleCSharpTypeDatatype : Datatype {

        public SimpleCSharpTypeDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
        }

        public override string GetReader() {
            return $"((Func<PacketBuffer, {CSharpType}>)((buffer) => buffer.Read{TypeName}()))";
        }
        public override string GetWriter() {
            return $"((Action<PacketBuffer, {CSharpType}>)((buffer, value) => buffer.Write{TypeName}(value)))";
        }

        protected override void WriteStructure(CodeGenerator codeGenerator, Datatype? parent) { }
    }


    internal abstract class DatatypeGenerator {

        public ProtoCompiler Compiler;

        public abstract string TypeName { get; }
        public abstract bool IsGeneric { get; }

        public DatatypeGenerator(ProtoCompiler compiler) {
            this.Compiler = compiler;
        }

        public abstract Datatype Create(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? parent, StructureDatatype? outerStructure);

        public abstract void WriteClassReader(CodeGenerator codeGenerator);
        public abstract void WriteClassWriter(CodeGenerator codeGenerator);
    }

    internal abstract class DatatypeGenerator<T> : DatatypeGenerator where T : Datatype {

        public DatatypeGenerator(ProtoCompiler compiler) : base(compiler) { }

        public override Datatype Create(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? parent, StructureDatatype? outerStructure) {
            var type = typeof(T);
            return (Datatype)Activator.CreateInstance(type, new object?[] { compiler, options, name, parent, outerStructure })!;
        }
    }

    internal abstract class StructureDatatype : Datatype {

        internal Dictionary<string, Field> RequiredParentFields = new Dictionary<string, Field>();
        protected StructureDatatype(ProtoCompiler compiler, JToken? options, string name, ContainerDatatype? container, StructureDatatype? outerStructure) : base(compiler, options, name, container, outerStructure) {
            if (options == null) throw new Exception();
        }

        internal class Field {
            public string Name;
            public Datatype Type;

            public Field(string name, Datatype type) {
                Name = name;
                Type = type;
            }

            public static Field Parse(JObject obj, ContainerDatatype current) {


                string? name = (string?)obj.GetValue("name");
                if (name == null) {
                    name = "Anon";
                }

                var type = Datatype.Parse(current.Compiler, obj.GetValue("type")!, name, current, current);

                return new Field(name, type);
            }
        }
    }
}

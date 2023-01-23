using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Generator
{
    internal class CodeGenerator
    {

        public const string Indent = "\t";
        public const string NewLine = "\r\n";

        private readonly StringBuilder Builder;

        private int IndentCount;

        private int ScopeStack;

        public CodeGenerator()
        {
            this.Builder = new StringBuilder();
        }
        public string CurrentIndent => string.Join("", Enumerable.Repeat(Indent, this.IndentCount));

        public void ClearIndent() => this.IndentCount = 0;
        public void PushIndent() => this.IndentCount++;
        public void PopIndent()
        {
            this.IndentCount--;
            if (this.IndentCount < 0) throw new Exception();
        }

        public void WriteLine()
        {
            this.Builder.AppendLine();
        }
        public void WriteLine(string line)
        {
            this.Builder.AppendLine(this.CurrentIndent + line);
        }

        public void WriteBlock(string str)
        {
            var lines = str.Split(Environment.NewLine);
            foreach (var line in lines)
            {
                this.WriteLine(line);
            }
        }

        public override string ToString() => this.Builder.ToString();
        public void Begin(string line)
        {
            this.WriteLine(line);
            this.WriteLine("{");
            this.ScopeStack++;
            this.PushIndent();
        }

        public void Finish(bool semicolon = false)
        {
            if (this.ScopeStack <= 0) throw new Exception();
            this.PopIndent();
            this.WriteLine("}" + (semicolon ? ";" : ""));
            this.ScopeStack--;
        }

        public void CommentBlock(string comment)
        {
            this.WriteLine(new string(Enumerable.Repeat('/', comment.Length + 10).ToArray()));
            this.WriteLine($"//   {comment}   //");
            this.WriteLine(new string(Enumerable.Repeat('/', comment.Length + 10).ToArray()));
        }
    }


    internal class InfoGeneratorTemplate<T>
    {
        public required string Name;
        public required string Namespace;
        public required T[] Data;
        public required Dictionary<string, Func<object, string>> Stringifiers;
        public required Func<T, string> NameGenerator;
        public required Func<T, int> Indexer;
        public EnumGenerator<T>[]? Indexers;
        public Func<T, string>[]? AdditionalInfoArgs;
        public Action<T[], CodeGenerator>? GenerateCodeBlock;
    }

    internal class InfoGenerator<T>
    {
        static NumberFormatInfo nfi = new NumberFormatInfo() {
            NumberDecimalSeparator = "."
        };

        private static readonly Dictionary<string, Func<object, string>> nativeStringifiers = new Dictionary<string, Func<object, string>>() {
            { typeof(int).FullName!,  (x) => x.ToString()! },
            { typeof(double).FullName!, (x)=> ((double)x).ToString(nfi)! + "D" },
            { typeof(float).FullName!, (x)=> ((float)x).ToString(nfi)! + "F" },
            { typeof(string).FullName!, (x) => $@"""{x}""" },
            { typeof(bool).FullName!, (x) => $@"{x.ToString()!.ToLower()}" },
        };

        public static string StringifyDefaults(object value)
        {
            return nativeStringifiers[value.GetType().FullName!](value);
        }

        public static string StringifyArray<U>(U[] array, Func<U, string> stringifier)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"new {GetTypeStr(typeof(U))}[] {{ ");

            foreach (var u in array)
            {
                sb.Append(stringifier(u));
                sb.Append(", ");
            }
            if (array.Length > 0)
            {
                sb.Remove(sb.Length - 2, 2);
            }
            sb.Append(" }");
            return sb.ToString();
        }

        private InfoGeneratorTemplate<T> Template;
        public InfoGenerator(InfoGeneratorTemplate<T> template)
        {
            this.Template = template;
            if (this.Template.Indexers == null)
            {
                this.Template.Indexers = new EnumGenerator<T>[0];
            }

            Array.Resize(ref this.Template.Indexers, this.Template.Indexers.Length + 1);
            this.Template.Indexers[^1] = new EnumGenerator<T>()
            {
                GetName = (val) => template.NameGenerator(val),
                Name = this.Template.Name
            };
        }

        internal void GenerateInfos(CodeGenerator codeGenerator)
        {
            codeGenerator.Begin($"namespace {this.Template.Namespace}");
            codeGenerator.Begin($"public static class {this.Template.Name}Palette");

            var props = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance)
                                .Where(x => x.GetCustomAttribute<IndexAttribute>() != null)
                                 .OrderBy(x => x.GetCustomAttribute<IndexAttribute>()!.Index).ToArray();

            if (this.Template.GenerateCodeBlock != null)
            {
                this.Template.GenerateCodeBlock(this.Template.Data, codeGenerator);
            }

            foreach (var data in this.Template.Data)
            {
                var csharpName = this.Template.NameGenerator(data);
                StringBuilder sb = new StringBuilder();
                sb.Append($"public static readonly {this.Template.Name}Info {csharpName}Info = new {this.Template.Name}Info(");
                sb.Append(string.Join(", ", props.Select(x => this.Template.Stringifiers[x.Name](x.GetValue(data)!))));
                if (this.Template.AdditionalInfoArgs != null && this.Template.AdditionalInfoArgs.Length > 0)
                {
                    sb.Append(", ");
                    sb.Append(string.Join(", ", this.Template.AdditionalInfoArgs.Select(x => x(data))));
                }
                sb.Append(");");
                codeGenerator.WriteLine(sb.ToString()); 
            }

            codeGenerator.Begin($"public static {Template.Name}Info Get{Template.Name}InfoById(int id) => id switch");
            foreach (var data in this.Template.Data)
                codeGenerator.WriteLine($"{this.Template.Indexer(data)} => {this.Template.NameGenerator(data)}Info,");
            codeGenerator.WriteLine(@"_ => throw new ArgumentException($""Biome with id {id} not found!"")");
            codeGenerator.Finish(semicolon: true);

            codeGenerator.Finish();

            foreach (var enumGen in this.Template.Indexers!)
            {
                enumGen.Feed(this.Template.Data);
                codeGenerator.Begin($"public enum {enumGen.Name}Type");
                foreach (var entry in enumGen.Values)
                {
                    codeGenerator.WriteLine($"{entry.Key} = {entry.Value},");
                }
                codeGenerator.Finish();
            }

            codeGenerator.WriteBlock($@"
public static class {this.Template.Name}Extensions 
{{
    public static {this.Template.Name}Info GetInfo(this {this.Template.Name}Type type)
    {{
        return {this.Template.Name}Palette.Get{this.Template.Name}InfoById((int)type);
    }}
}}");

            codeGenerator.Finish();
        }

        private static string GetTypeStr(Type type)
        {
            return type.FullName switch {
                "System.Int32" => "int",
                "System.Int64" => "long",
                "System.Single" => "float",
                "System.Double" => "double",
                _ => type.Name
            };
        }
    }

    public class IndexAttribute : Attribute
    {
        public int Index { get; }
        public IndexAttribute(int index) {
            this.Index = index;
        }
    }

    public class EnumGenerator<T>
    {
        public Dictionary<string, int> Values = new Dictionary<string, int>();
        public required Func<T, string> GetName;
        public required string Name;
        public Func<T, int>? Indexer;
        
        public void Feed(T[] data)
        {
            foreach (T entry in data)
            {
                var name = GetName(entry);
                if (Values.TryGetValue(name, out var id))
                {
                    continue;
                }

                if (Indexer != null)
                {
                    id = Indexer(entry);
                }
                else
                {
                    id = Values.Values.Count;
                }

                Values.Add(name, id);
            }
        }
    }
}

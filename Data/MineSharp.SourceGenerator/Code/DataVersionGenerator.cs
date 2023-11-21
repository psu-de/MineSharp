using Humanizer;
using Newtonsoft.Json.Linq;
using System.Text;

namespace MineSharp.SourceGenerator.Code;

public class DataVersionGenerator
{
    public required string Namespace { get; init; }
    public required string ClassName { get; init; }
    public required string EnumName { get; init; }
    public required string InfoClass { get; init; }
    public required string Outfile { get; init; }
    public required Func<JToken, string> Stringify { get; init; }
    public required Func<JToken, string> KeySelector { get; init; }
    public required JToken[] Properties { get; init; }
    public required string[] Usings { get; init; }

    public Task Write()
    {
        var sb = new StringBuilder();
        foreach (var @using in this.Usings)
        {
            sb.AppendLine($"using {@using};");
        }
        sb.AppendLine();
        sb.AppendLine($"namespace {this.Namespace};");
        sb.AppendLine();
        sb.AppendLine($"internal class {this.ClassName} : DataVersion<{this.EnumName}, {this.InfoClass}>");
        sb.AppendLine("{");
        sb.AppendLine($"    private static Dictionary<{this.EnumName}, {this.InfoClass}> Values {{ get; }} = new Dictionary<{this.EnumName}, {this.InfoClass}>()");
        sb.AppendLine($"    {{");
        foreach (var token in this.Properties)
        {
            sb.AppendLine($"        {{ {this.EnumName}.{this.KeySelector(token)}, {this.Stringify(token)} }},");
        }
        sb.AppendLine($"    }};");
        sb.AppendLine($"    public override Dictionary<{this.EnumName}, {this.InfoClass}> Palette => Values;");
        sb.AppendLine("}");

        return File.WriteAllTextAsync(this.Outfile, sb.ToString());
    }
}

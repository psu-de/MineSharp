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
        var writer = new CodeWriter();

        writer.Disclaimer();

        foreach (var @using in this.Usings)
        {
            writer.WriteLine($"using {@using};");
        }
        
        writer.WriteLine();
        writer.WriteLine($"namespace {this.Namespace};");
        writer.WriteLine();
        writer.Begin($"internal class {this.ClassName} : DataVersion<{this.EnumName}, {this.InfoClass}>");
        writer.Begin($"private static Dictionary<{this.EnumName}, {this.InfoClass}> Values {{ get; }} = new()");
        foreach (var token in this.Properties)
        {
            writer.WriteLine($"{{ {this.EnumName}.{this.KeySelector(token)}, {this.Stringify(token)} }},");
        }
        writer.Finish(semicolon: true);
        writer.WriteLine($"public override Dictionary<{this.EnumName}, {this.InfoClass}> Palette => Values;");
        writer.Finish();

        return File.WriteAllTextAsync(this.Outfile, writer.ToString());
    }
}

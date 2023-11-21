using System.Text;

namespace MineSharp.SourceGenerator.Code;

public class EnumGenerator
{
    public required string ClassName { get; init; }
    public required string Namespace { get; init; }
    public required string Outfile { get; init; }
    public required Dictionary<string, int> Entries { get; init; }

    public Task Write()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"namespace {this.Namespace};");
        sb.AppendLine();
        sb.AppendLine($"public enum {this.ClassName}");
        sb.AppendLine("{");
        foreach (var entry in this.Entries)
            sb.AppendLine($"    {entry.Key} = {entry.Value},");
        sb.AppendLine("}");
        return File.WriteAllTextAsync(this.Outfile, sb.ToString());
    }
}

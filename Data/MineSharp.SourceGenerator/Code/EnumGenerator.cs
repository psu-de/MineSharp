namespace MineSharp.SourceGenerator.Code;

public class EnumGenerator
{
    public required string ClassName { get; init; }
    public required string Namespace { get; init; }
    public required string Outfile { get; init; }
    public required Dictionary<string, int> Entries { get; init; }

    public Task Write()
    {
        var writer = new CodeWriter();
        writer.Disclaimer();
        writer.WriteLine($"namespace {Namespace};");
        writer.WriteLine();
        writer.Begin($"public enum {ClassName}");
        foreach (var entry in Entries)
        {
            writer.WriteLine($"{entry.Key} = {entry.Value},");
        }

        writer.Finish();
        return File.WriteAllTextAsync(Outfile, writer.ToString());
    }
}

using MineSharp.SourceGenerator.Code;
using MineSharp.SourceGenerator.Utils;
using Newtonsoft.Json.Linq;

namespace MineSharp.SourceGenerator.Generators;

public class LanguageGenerator : IGenerator
{
    public string Name => "Language";
    
    public async Task Run(MinecraftDataWrapper wrapper)
    {
        foreach (var version in Config.IncludedVersions)
        {
            await GenerateVersion(wrapper, version);
        }
    }

    private async Task GenerateVersion(MinecraftDataWrapper wrapper, string version)
    {
        var path = wrapper.GetPath(version, "language");
        if (VersionMapGenerator.GetInstance().IsRegistered("language", path))
        {
            VersionMapGenerator.GetInstance().RegisterVersion("language", version, path);
            return;
        }
        
        VersionMapGenerator.GetInstance().RegisterVersion("language", version, path);
        
        var outdir = DirectoryUtils.GetDataSourceDirectory(Path.Join("Language", "Versions"));
        var v = path.Replace("pc/", "").Replace(".", "_");
        var language = await wrapper.Parse(version, "language");

        var writer = new CodeWriter();
        writer.WriteLine();
        writer.WriteLine("using MineSharp.Data.Language;");
        writer.WriteLine();
        writer.WriteLine("namespace MineSharp.Data.Language.Versions;");
        writer.WriteLine();
        writer.Begin($"internal class Language_{v} : LanguageVersion");
        writer.Begin("public override Dictionary<string, string> Translations { get; } = new()");
        foreach (var prop in ((JObject)language).Properties())
        {
            writer.WriteLine($"{{ {Str.String(prop.Name)}, {Str.String(Sanitize((string)prop.Value!))} }},");
        }

        writer.Finish(semicolon: true);
        writer.Finish();
        
        await File.WriteAllTextAsync(
            Path.Join(outdir, $"Language_{v}.cs"), writer.ToString());
    }

    private static string Sanitize(string msg)
    {
        return msg.Replace("\\", "\\\\")
            .Replace("\n", "\\n")
            .Replace("\"", "\\\"");
    }
}
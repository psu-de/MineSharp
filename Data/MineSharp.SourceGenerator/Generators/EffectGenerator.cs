using Humanizer;
using MineSharp.SourceGenerator.Code;
using MineSharp.SourceGenerator.Utils;
using Newtonsoft.Json.Linq;

namespace MineSharp.SourceGenerator.Generators;

public class EffectGenerator : IGenerator
{
    public string Name => "Effect";

    public async Task Run(MinecraftDataWrapper wrapper)
    {
        await GenerateEnum(wrapper);

        foreach (var version in Config.IncludedVersions)
        {
            await GenerateVersion(wrapper, version);
        }
    }
    
    private async Task GenerateVersion(MinecraftDataWrapper wrapper, string version)
    {
        var path = wrapper.GetPath(version, "effects");
        if (VersionMapGenerator.GetInstance().IsRegistered("effects", path))
        {
            VersionMapGenerator.GetInstance().RegisterVersion("effects", version, path);
            return;
        }
        
        VersionMapGenerator.GetInstance().RegisterVersion("effects", version, path);
        
        var outdir = DirectoryUtils.GetDataSourceDirectory("Effects\\Versions");
        var v = path.Replace("pc/", "").Replace(".", "_");
        var effects = await wrapper.GetEffects(version);

        await new DataVersionGenerator() {
            Namespace = "MineSharp.Data.Effects.Versions",
            ClassName = $"Effects_{v}",
            EnumName = "EffectType",
            InfoClass = "EffectInfo",
            Usings = new[] { "MineSharp.Core.Common.Effects" },
            Outfile = Path.Join(outdir, $"Effects_{v}.cs"),
            Properties = ((JArray)effects).ToArray(),
            Stringify = Stringify,
            KeySelector = KeySelector
        }.Write();
    }
    
    private async Task GenerateEnum(MinecraftDataWrapper wrapper)
    {
        var outdir = DirectoryUtils.GetCoreSourceDirectory("Common\\Effects");
        var effects = await wrapper.GetEffects(Config.LatestVersion);

        var effectValues = new Dictionary<string, int>();

        foreach (var effect in (JArray)effects)
        {
            effectValues.Add(((string)effect.SelectToken("name")!).Pascalize(), (int)effect.SelectToken("id")!);
        }

        await new EnumGenerator() {
            Namespace = "MineSharp.Core.Common.Effects",
            ClassName = "EffectType",
            Outfile = Path.Join(outdir, "EffectType.cs"),
            Entries = effectValues
        }.Write();
    }
    
    private string KeySelector(JToken token)
    {
        return ((string)token.SelectToken("name")!).Pascalize();
    }

    private string Stringify(JToken token)
    {
        var id = (int)token.SelectToken("id")!;
        var name = (string)token.SelectToken("name")!;
        var displayName = (string)token.SelectToken("displayName")!;
        var isGood = (string)token.SelectToken("type")! == "good";
        
        return $"new EffectInfo({id}, " +
               $"EffectType.{name.Pascalize()}, " +
               $"{Str.String(name)}, " +
               $"{Str.String(displayName)}, " +
               $"{Str.Bool(isGood)})";
    }
}

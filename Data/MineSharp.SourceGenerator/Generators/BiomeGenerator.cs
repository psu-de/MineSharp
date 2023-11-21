using Humanizer;
using MineSharp.SourceGenerator.Code;
using MineSharp.SourceGenerator.Utils;
using Newtonsoft.Json.Linq;

namespace MineSharp.SourceGenerator.Generators;

public class BiomeGenerator : IGenerator
{
    public string Name => "Biome";
    
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
        var path = wrapper.GetPath(version, "biomes");
        if (VersionMapGenerator.GetInstance().IsRegistered("biomes", path))
        {
            VersionMapGenerator.GetInstance().RegisterVersion("biomes", version, path);
            return;
        }
        
        VersionMapGenerator.GetInstance().RegisterVersion("biomes", version, path);
        
        var outdir = DirectoryUtils.GetDataSourceDirectory("Biomes\\Versions");
        var v = path.Replace("pc/", "").Replace(".", "_");
        var biomes = await wrapper.GetBiomes(version);

        await new DataVersionGenerator() {
            Namespace = "MineSharp.Data.Biomes.Versions",
            ClassName = $"Biomes_{v}",
            EnumName = "BiomeType",
            InfoClass = "BiomeInfo",
            Usings = new[] { "MineSharp.Core.Common", "MineSharp.Core.Common.Biomes" },
            Outfile = Path.Join(outdir, $"Biomes_{v}.cs"),
            Properties = ((JArray)biomes).ToArray(),
            Stringify = Stringify,
            KeySelector = KeySelector
        }.Write();
    }

    private async Task GenerateEnum(MinecraftDataWrapper wrapper)
    {
        var outdir = DirectoryUtils.GetCoreSourceDirectory("Common\\Biomes");
        var biomes = await wrapper.GetBiomes(Config.LatestVersion);

        var biomeValues = new Dictionary<string, int>();
        var biomeCategories = new HashSet<string>();

        foreach (var biome in (JArray)biomes)
        {
            biomeValues.Add(((string)biome.SelectToken("name")!).Pascalize(), (int)biome.SelectToken("id")!);
            biomeCategories.Add(((string)biome.SelectToken("category")!).Pascalize());
        }

        await new EnumGenerator() {
            Namespace = "MineSharp.Core.Common.Biomes",
            ClassName = "BiomeType",
            Outfile = Path.Join(outdir, "BiomeType.cs"),
            Entries = biomeValues
        }.Write();
        
        await new EnumGenerator() {
            Namespace = "MineSharp.Core.Common.Biomes",
            ClassName = "BiomeCategory",
            Outfile = Path.Join(outdir, "BiomeCategory.cs"),
            Entries = biomeCategories
                .Select((x, i) => (x, i))
                .ToDictionary(x => x.x, x => x.i)
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
        var category = GetCategory(token.SelectToken("category")!);
        var temperature = (float)token.SelectToken("temperature")!;
        var dimension = (string)token.SelectToken("dimension")!;
        var color = (int)token.SelectToken("color")!;


        bool precipitation;
        if (null == token.SelectToken("has_precipitation"))
            precipitation = "none" == (string)token.SelectToken("precipitation")!;
        else precipitation = (bool)token.SelectToken("has_precipitation")!;

        return $"new BiomeInfo({id}, " +
               $"{Str.String(name)}, " +
               $"{Str.String(displayName)}, " +
               $"BiomeCategory.{category}, " +
               $"{Str.Float(temperature)}, " +
               $"{Str.Bool(precipitation)}, " +
               $"Dimension.{dimension.Pascalize()}, " +
               $"{color})";
    }

    private string GetCategory(JToken token)
    {
        var str = (string)token!;
        if (str == "icy")
            str = "ice";
        return str.Pascalize();
    }
}

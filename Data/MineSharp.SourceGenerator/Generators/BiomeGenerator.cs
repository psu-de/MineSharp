using Humanizer;
using MineSharp.SourceGenerator.Code;
using MineSharp.SourceGenerator.Generators.Core;
using MineSharp.SourceGenerator.Utils;
using Newtonsoft.Json.Linq;

namespace MineSharp.SourceGenerator.Generators;

public class BiomeGenerator : CommonGenerator
{
    protected override string Singular => "Biome";
    protected override string Namespace => "Biomes";
    protected override string DataKey => "biomes";
    protected override string[] ExtraUsings { get; } = { "MineSharp.Core.Common" };


    private async Task GenerateEnum(MinecraftDataWrapper wrapper)
    {
        var outdir = DirectoryUtils.GetCoreSourceDirectory(Path.Join("Common", "Biomes"));
        var biomes = await wrapper.GetBiomes(Config.LatestVersion);
        var biomeCategories = new HashSet<string>();

        foreach (var biome in (JArray)biomes)
        {
            biomeCategories.Add(GetCategory(biome));
        }
        
        await new EnumGenerator() {
            Namespace = "MineSharp.Core.Common.Biomes",
            ClassName = "BiomeCategory",
            Outfile = Path.Join(outdir, "BiomeCategory.cs"),
            Entries = biomeCategories
                .Select((x, i) => (x, i))
                .ToDictionary(x => x.x, x => x.i)
        }.Write();
    }

    protected override async Task WriteAdditionalItems(MinecraftDataWrapper wrapper)
    {
        var outdir = DirectoryUtils.GetCoreSourceDirectory(Path.Join("Common", "Biomes"));
        var biomes = await wrapper.GetBiomes(Config.LatestVersion);

        var biomeCategories = new HashSet<string>();

        foreach (var biome in (JArray)biomes)
        {
            biomeCategories.Add(((string)biome.SelectToken("category")!).Pascalize());
        }
        
        await new EnumGenerator() {
            Namespace = "MineSharp.Core.Common.Biomes",
            ClassName = "BiomeCategory",
            Outfile = Path.Join(outdir, "BiomeCategory.cs"),
            Entries = biomeCategories
                .Select((x, i) => (x, i))
                .ToDictionary(x => x.x, x => x.i)
        }.Write();
    }

    protected override JToken[] GetProperties(JToken data)
        => ((JArray)data).ToArray();
    
    protected override string GetName(JToken token)
    {
        var name = (string)token.SelectToken("name")!;
        return NameUtils.GetBiomeName(name);
    }

    protected override string Stringify(JToken token)
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
               $"BiomeType.{name.Pascalize()}, " +
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

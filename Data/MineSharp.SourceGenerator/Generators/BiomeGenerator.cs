using MineSharp.SourceGenerator.Utils;
using Newtonsoft.Json.Linq;

namespace MineSharp.SourceGenerator.Generators;

public class BiomeGenerator
{
    private readonly Generator typeGenerator =
        new Generator("biomes", GetName, "BiomeType", "Biomes");

    private readonly Generator categoryGenerator =
        new Generator("biomes", GetCategoryName, "BiomeCategory", "Biomes");

    public Task Run(MinecraftDataWrapper wrapper)
    {
        return Task.WhenAll(
            typeGenerator.Generate(wrapper),
            categoryGenerator.Generate(wrapper));
    }
    
    private static string GetName(JToken token)
    {
        var name = (string)token.SelectToken("name")!;
        return NameUtils.GetBiomeName(name);
    }

    private static string GetCategoryName(JToken token)
    {
        var name = (string)token.SelectToken("category")!;
        return NameUtils.GetBiomeCategory(name);
    }
}
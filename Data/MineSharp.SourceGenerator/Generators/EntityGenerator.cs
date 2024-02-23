using MineSharp.SourceGenerator.Utils;
using Newtonsoft.Json.Linq;

namespace MineSharp.SourceGenerator.Generators;

public class EntityGenerator
{
    private readonly Generator typeGenerator =
        new Generator("entities", GetName, "EntityType", "Entities");

    private readonly Generator categoryGenerator =
        new Generator("entities", GetCategoryName, "EntityCategory", "Entities");

    public Task Run(MinecraftDataWrapper wrapper)
    {
        return Task.WhenAll(
            typeGenerator.Generate(wrapper),
            categoryGenerator.Generate(wrapper));
    }

    private static string GetName(JToken token)
    {
        var name = (string)token.SelectToken("name")!;
        return NameUtils.GetEntityName(name);
    }

    private static string GetCategoryName(JToken token)
    {
        var name = (string)token.SelectToken("category")!;
        return NameUtils.GetEntityCategory(name);
    }
}

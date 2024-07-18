using MineSharp.SourceGenerator.Utils;
using Newtonsoft.Json.Linq;

namespace MineSharp.SourceGenerator.Generators;

public class EntityGenerator
{
    private readonly Generator categoryGenerator = new("entities", GetCategoryName, "EntityCategory", "Entities");

    private readonly Generator typeGenerator = new("entities", GetName, "EntityType", "Entities");

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

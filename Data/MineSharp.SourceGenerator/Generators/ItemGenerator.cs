using Humanizer;
using MineSharp.SourceGenerator.Code;
using MineSharp.SourceGenerator.Generators.Core;
using MineSharp.SourceGenerator.Utils;
using Newtonsoft.Json.Linq;

namespace MineSharp.SourceGenerator.Generators;

public class ItemGenerator : CommonGenerator
{
    protected override string DataKey => "items";
    protected override string Namespace => "Items";
    protected override string Singular => "Item";
    protected override string[] ExtraUsings { get; } = { "MineSharp.Core.Common.Enchantments" };

    protected override JToken[] GetProperties(JToken data)
        => ((JArray)data).ToArray();
    
    protected override string GetName(JToken token)
    {
        var name = (string)token.SelectToken("name")!;
        return NameUtils.GetItemName(name);
    }
    
    protected override string Stringify(JToken token)
    {
        var id = (int)token.SelectToken("id")!;
        var name = (string)token.SelectToken("name")!;
        var displayName = (string)token.SelectToken("displayName")!;
        var stackSize = (int)token.SelectToken("stackSize")!;
        var maxDurability = ((int?)token.SelectToken("maxDurability"))?.ToString() ?? "null";
        var enchantCategories = this.GetEnchantmentCategories(token.SelectToken("enchantCategories"));
        var repairWith = GetRepairWith(token.SelectToken("repairWith"));

        return $"new ItemInfo({id}, " +
               $"ItemType.{NameUtils.GetItemName(name)}, " +
               $"{Str.String(name)}, " +
               $"{Str.String(displayName)}, " +
               $"{stackSize}, " +
               $"{maxDurability}, " +
               $"{enchantCategories}, " +
               $"{repairWith})";
    }

    private string GetEnchantmentCategories(JToken? token)
    {
        if (null == token)
            return "null";
        
        var list = new List<string>();
        foreach (var item in (JArray)token)
        {
            list.Add(((string)item!).Pascalize());
        }
        
        if (list.Count == 0)
            return "Array.Empty<EnchantmentCategory>()";

        return $"new [] {{ {string.Join(", ", list.Select(x => $"EnchantmentCategory.{x}"))} }}";
    }

    private string GetRepairWith(JToken? token)
    {
        if (null == token)
            return "null";
        
        var list = new List<string>();
        foreach (var item in (JArray)token)
        {
            list.Add(((string)item!).Pascalize());
        }

        if (list.Count == 0)
            return $"Array.Empty<ItemType>()";

        return $"new [] {{ {string.Join(", ", list.Select(x => $"ItemType.{x}"))} }}";
    }
}

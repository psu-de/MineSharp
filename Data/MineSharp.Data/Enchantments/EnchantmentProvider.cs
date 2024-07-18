using MineSharp.Core.Common.Enchantments;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;
using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Enchantments;

internal class EnchantmentProvider : IDataProvider<EnchantmentInfo[]>
{
    private static readonly EnumNameLookup<EnchantmentType> EnchantmentTypeLookup = new();
    private static readonly EnumNameLookup<EnchantmentCategory> EnchantmentCategoryLookup = new();

    private readonly JArray token;

    public EnchantmentProvider(JToken token)
    {
        if (token.Type != JTokenType.Array)
        {
            throw new ArgumentException("Expected token to be an array");
        }

        this.token = (JArray)token;
    }

    public EnchantmentInfo[] GetData()
    {
        var data = new EnchantmentInfo[token.Count];

        for (var i = 0; i < token.Count; i++)
        {
            data[i] = FromToken(token[i]);
        }

        return data;
    }

    private static EnchantmentInfo FromToken(JToken token)
    {
        var id = (int)token.SelectToken("id")!;
        var name = (string)token.SelectToken("name")!;
        var displayName = (string)token.SelectToken("displayName")!;
        var maxLevel = (int)token.SelectToken("maxLevel")!;
        var minCost = (JObject)token.SelectToken("minCost")!;
        var maxCost = (JObject)token.SelectToken("maxCost")!;
        var treasureOnly = (bool)token.SelectToken("treasureOnly")!;
        var curse = (bool)token.SelectToken("curse")!;
        var exclude = (JArray)token.SelectToken("exclude")!;
        var category = (string)token.SelectToken("category")!;
        var weight = (int)token.SelectToken("weight")!;
        var tradeable = (bool)token.SelectToken("tradeable")!;
        var discoverable = (bool)token.SelectToken("discoverable")!;

        return new(
            id,
            EnchantmentTypeLookup.FromName(NameUtils.GetEnchantmentName(name)),
            name,
            displayName,
            maxLevel,
            GetEnchantmentCost(minCost),
            GetEnchantmentCost(maxCost),
            treasureOnly,
            curse,
            GetExclusions(exclude),
            EnchantmentCategoryLookup.FromName(NameUtils.GetEnchantmentCategory(category)),
            weight,
            tradeable,
            discoverable);
    }

    private static EnchantCost GetEnchantmentCost(JObject obj)
    {
        var a = (int)obj.SelectToken("a")!;
        var b = (int)obj.SelectToken("b")!;

        return new(a, b);
    }

    private static EnchantmentType[] GetExclusions(JArray array)
    {
        if (array.Count == 0)
        {
            return Array.Empty<EnchantmentType>();
        }

        return array
              .ToObject<string[]>()!
              .Select(NameUtils.GetEnchantmentName)
              .Select(EnchantmentTypeLookup.FromName)
              .ToArray();
    }
}

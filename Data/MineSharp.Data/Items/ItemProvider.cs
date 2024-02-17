using Humanizer;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Enchantments;
using MineSharp.Core.Common.Items;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;
using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Items;

internal class ItemProvider : IDataProvider<ItemInfo[]>
{
    private static readonly EnumNameLookup<ItemType> ItemTypeLookup = new();
    private static readonly EnumNameLookup<EnchantmentCategory> EnchantmentCategoryLookup = new();

    private readonly JArray token;
    
    public ItemProvider(JToken token)
    {
        if (token.Type != JTokenType.Array)
        {
            throw new InvalidOperationException("Expected the token to be an array");
        }
        
        this.token = (token as JArray)!;
    }
    
    public ItemInfo[] GetData()
    {
        var length = token.Count;
        var data = new ItemInfo[length];

        for (int i = 0; i < length; i++)
        {
            data[i] = FromToken(token[i]!);
        }

        return data;
    }

    private static ItemInfo FromToken(JToken dataToken)
    {
        var id = (int)dataToken.SelectToken("id")!;
        var name = (string)dataToken.SelectToken("name")!;
        var displayName = (string)dataToken.SelectToken("displayToken")!;
        var stackSize = (int)dataToken.SelectToken("stackSize")!;
        var durability = (int?)dataToken.SelectToken("maxDurability")!;
        var enchantments = (JArray?)dataToken.SelectToken("enchantCategories");
        var repairWith = (JArray?)dataToken.SelectToken("repairWith")!;

        return new ItemInfo(
            id,
            ItemTypeLookup.FromName(name),
            name,
            displayName,
            stackSize,
            durability,
            GetEnchantments(enchantments),
            GetRepairItems(repairWith)
        );
    }

    private static EnchantmentCategory[] GetEnchantments(JArray? enchantments)
    {
        if (enchantments == null)
            return Array.Empty<EnchantmentCategory>();

        return enchantments
            .ToObject<string[]>()!
            .Select(NameUtils.GetEnchantmentName)
            .Select(EnchantmentCategoryLookup.FromName)
            .ToArray();
    }

    private static ItemType[] GetRepairItems(JArray? repairWith)
    {
        if (repairWith == null)
            return Array.Empty<ItemType>();

        return repairWith
            .ToObject<string[]>()!
            .Select(NameUtils.GetItemName)
            .Select(ItemTypeLookup.FromName)
            .ToArray();
    }
}
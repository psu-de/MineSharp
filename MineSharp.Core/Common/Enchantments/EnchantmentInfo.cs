namespace MineSharp.Core.Common.Enchantments;

/// <summary>
///     Descriptor for enchantments
/// </summary>
/// <param name="Id">The numerical id of the enchantment (depends on minecraft version)</param>
/// <param name="Type">The <see cref="EnchantmentType" /> of this enchantment (independent on minecraft version)</param>
/// <param name="Name">The text id of this enchantment</param>
/// <param name="DisplayName">Minecraft's display name of this enchantment</param>
/// <param name="MaxLevel">The highest possible level of this enchantment</param>
/// <param name="MinCost"></param>
/// <param name="MaxCost"></param>
/// <param name="TreasureOnly">Whether this enchantment can only be found in chests</param>
/// <param name="Curse">Whether this enchantment is a curse</param>
/// <param name="Exclude">An array of other enchantment types, which can't be applied to tool when it has this enchantment</param>
/// <param name="Category"></param>
/// <param name="Weight"></param>
/// <param name="Tradeable"></param>
/// <param name="Discoverable"></param>
public record EnchantmentInfo(
    int Id,
    EnchantmentType Type,
    string Name,
    string DisplayName,
    int MaxLevel,
    EnchantCost MinCost,
    EnchantCost MaxCost,
    bool TreasureOnly,
    bool Curse,
    EnchantmentType[] Exclude,
    EnchantmentCategory Category,
    int Weight,
    bool Tradeable,
    bool Discoverable);

/// <summary>
///     Enchantment cost
/// </summary>
/// <param name="A"></param>
/// <param name="B"></param>
public record EnchantCost(int A, int B);

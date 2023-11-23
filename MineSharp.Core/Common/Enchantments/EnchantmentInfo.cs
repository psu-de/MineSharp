namespace MineSharp.Core.Common.Enchantments;

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

public record EnchantCost(int A, int B);
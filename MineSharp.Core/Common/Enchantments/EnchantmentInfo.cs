namespace MineSharp.Core.Common.Enchantments;

public record EnchantmentInfo(
    int Id,
    string Name,
    string DisplayName,
    int MaxLevel,
    EnchantCost MinCost,
    EnchantCost MaxCost,
    bool TreasureOnly,
    bool Curse,
    string[] Exclude,
    string Category,
    int Weight,
    bool Tradeable,
    bool Discoverable);

public record EnchantCost(int A, int B);
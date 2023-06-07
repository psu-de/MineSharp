namespace MineSharp.Core.Common.Items;

public record ItemInfo(
    int Id,
    string Name,
    string DisplayName,
    int StackSize,
    int MaxDurability,
    string[] EnchantCategories,
    string[] RepairWith);

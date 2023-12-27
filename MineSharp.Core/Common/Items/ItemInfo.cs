using MineSharp.Core.Common.Enchantments;

namespace MineSharp.Core.Common.Items;

/// <summary>
/// Item Descriptor class
/// </summary>
/// <param name="Id">The numerical id of the item (depends on minecraft version)</param>
/// <param name="Type">The Type of this item (independent of minecraft version)</param>
/// <param name="Name">The text id of the item</param>
/// <param name="DisplayName">Minecraft's display name of the item</param>
/// <param name="StackSize">Max stack size of the item</param>
/// <param name="MaxDurability">Max durability of the item</param>
/// <param name="EnchantCategories">A list of enchantment that are available on the item</param>
/// <param name="RepairWith">A list of items that can repair the item</param>
public record ItemInfo(
    int Id,
    ItemType Type,
    string Name,
    string DisplayName,
    int StackSize,
    int? MaxDurability,
    EnchantmentCategory[]? EnchantCategories,
    ItemType[]? RepairWith);
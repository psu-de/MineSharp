using System.Collections.Frozen;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Data.Windows;

namespace MineSharp.Data.Mappings;

/// <summary>
/// Contains extra information about Windows
/// </summary>
public static class Windows
{
    /// <summary>
    /// A list of blocks that can be opened
    /// </summary>
    public static readonly IList<BlockType> OpenableBlocks = new List<BlockType>
    {
        BlockType.Chest,
        BlockType.TrappedChest,
        BlockType.EnderChest,
        BlockType.CraftingTable,
        BlockType.Furnace,
        BlockType.BlastFurnace,
        BlockType.Smoker,
        BlockType.Dispenser,
        BlockType.EnchantingTable,
        BlockType.BrewingStand,
        BlockType.Beacon,
        BlockType.Anvil,
        BlockType.Hopper,
        BlockType.ShulkerBox,
        BlockType.BlackShulkerBox,
        BlockType.BlueShulkerBox,
        BlockType.BrownShulkerBox,
        BlockType.CyanShulkerBox,
        BlockType.GrayShulkerBox,
        BlockType.GreenShulkerBox,
        BlockType.LightBlueShulkerBox,
        BlockType.LightGrayShulkerBox,
        BlockType.LimeShulkerBox,
        BlockType.MagentaShulkerBox,
        BlockType.OrangeShulkerBox,
        BlockType.PinkShulkerBox,
        BlockType.PurpleShulkerBox,
        BlockType.RedShulkerBox,
        BlockType.WhiteShulkerBox,
        BlockType.YellowShulkerBox,
        BlockType.CartographyTable,
        BlockType.Grindstone,
        BlockType.Lectern,
        BlockType.Loom,
        BlockType.Stonecutter
    };

    /// <summary>
    /// All windows known to MineSharp
    /// </summary>
    public static FrozenDictionary<Identifier, WindowInfo> WindowsInfos { get; } = new Dictionary<Identifier, WindowInfo>()
    {
        ["minecraft:generic_9x1"]   = new("minecraft:generic_9x1", "", 9),
        ["minecraft:generic_9x2"]   = new("minecraft:generic_9x2", "", 18),
        ["minecraft:generic_9x3"]   = new("minecraft:generic_9x3", "", 27),
        ["minecraft:generic_9x4"]   = new("minecraft:generic_9x4", "", 36),
        ["minecraft:generic_9x5"]   = new("minecraft:generic_9x5", "", 45),
        ["minecraft:generic_9x6"]   = new("minecraft:generic_9x6", "", 54),
        ["minecraft:generic_3x3"]   = new("minecraft:generic_3x3", "", 9),
        ["minecraft:crafter_3x3"]   = new("minecraft:crafter_3x3", "", 10),
        ["minecraft:anvil"]         = new("minecraft:anvil", "Anvil", 3),
        ["minecraft:beacon"]        = new("minecraft:beacon", "Beacon", 1),
        ["minecraft:blast_furnace"] = new("minecraft:blast_furnace", "Blast Furnace", 3),
        ["minecraft:brewing_stand"] = new("minecraft:brewing_stand", "Brewing stand", 5),
        ["minecraft:crafting"]      = new("minecraft:crafting", "Crafting table", 10),
        ["minecraft:enchantment"]   = new("minecraft:enchantment", "Enchantment table", 2),
        ["minecraft:furnace"]       = new("minecraft:furnace", "Furnace", 3),
        ["minecraft:grindstone"]    = new("minecraft:grindstone", "Grindstone", 3),
        ["minecraft:hopper"]        = new("minecraft:hopper", "Hopper or minecart with hopper", 5),
        ["minecraft:lectern"]       = new("minecraft:lectern", "Lectern", 1, true),
        ["minecraft:loom"]          = new("minecraft:loom", "Loom", 4),
        ["minecraft:merchant"]      = new("minecraft:merchant", "Villager, Wandering Trader", 3),
        ["minecraft:shulker_box"]   = new("minecraft:shulker_box", "Shulker box", 27),
        ["minecraft:smithing"]      = new("minecraft:smithing", "Smithing Table", 3),
        ["minecraft:smoker"]        = new("minecraft:smoker", "Smoker", 3),
        ["minecraft:cartography"]   = new("minecraft:cartography", "Cartography Table", 3),
        ["minecraft:stonecutter"]   = new("minecraft:stonecutter", "Stonecutter", 2)
    }.ToFrozenDictionary();
}

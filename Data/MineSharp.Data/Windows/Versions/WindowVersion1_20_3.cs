using MineSharp.Data.Framework.Providers;

namespace MineSharp.Data.Windows.Versions;

internal class WindowVersion1203 : IDataProvider<WindowInfo[]>
{
    private static readonly WindowInfo[] Windows =
    [
        new("minecraft:generic_9x1", "", 9),
        new("minecraft:generic_9x2", "", 18),
        new("minecraft:generic_9x3", "", 27),
        new("minecraft:generic_9x4", "", 36),
        new("minecraft:generic_9x5", "", 45),
        new("minecraft:generic_9x6", "", 54),
        new("minecraft:generic_3x3", "", 9),
        new("minecraft:crafter_3x3", "", 10),
        new("minecraft:anvil", "Anvil", 3),
        new("minecraft:beacon", "Beacon", 1),
        new("minecraft:blast_furnace", "Blast Furnace", 3),
        new("minecraft:brewing_stand", "Brewing stand", 5),
        new("minecraft:crafting", "Crafting table", 10),
        new("minecraft:enchantment", "Enchantment table", 2),
        new("minecraft:furnace", "Furnace", 3),
        new("minecraft:grindstone", "Grindstone", 3),
        new("minecraft:hopper", "Hopper or minecart with hopper", 5),
        new("minecraft:lectern", "Lectern", 1, true),
        new("minecraft:loom", "Loom", 4),
        new("minecraft:merchant", "Villager, Wandering Trader", 3),
        new("minecraft:shulker_box", "Shulker box", 27),
        new("minecraft:smithing", "Smithing Table", 3),
        new("minecraft:smoker", "Smoker", 3),
        new("minecraft:cartography", "Cartography Table", 3),
        new("minecraft:stonecutter", "Stonecutter", 2)
    ];

    public WindowInfo[] GetData()
    {
        return Windows;
    }
}

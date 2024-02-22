using MineSharp.Data.Framework.Providers;

namespace MineSharp.Data.Windows.Versions;

internal class WindowVersion1_16_1 : IDataProvider<WindowInfo[]>
{
    private static readonly WindowInfo[] Windows = [
        new WindowInfo("minecraft:generic_9x1", "", 9),
        new WindowInfo("minecraft:generic_9x2", "", 18),
        new WindowInfo("minecraft:generic_9x3", "", 27),
        new WindowInfo("minecraft:generic_9x4", "", 36),
        new WindowInfo("minecraft:generic_9x5", "", 45),
        new WindowInfo("minecraft:generic_9x6", "", 54),
        new WindowInfo("minecraft:generic_3x3", "", 9),
        new WindowInfo("minecraft:anvil", "Anvil", 3),
        new WindowInfo("minecraft:beacon", "Beacon", 1),
        new WindowInfo("minecraft:blast_furnace", "Blast Furnace", 3),
        new WindowInfo("minecraft:brewing_stand", "Brewing stand", 5),
        new WindowInfo("minecraft:crafting", "Crafting table", 10),
        new WindowInfo("minecraft:enchantment", "Enchantment table", 2),
        new WindowInfo("minecraft:furnace", "Furnace", 3),
        new WindowInfo("minecraft:grindstone", "Grindstone", 3),
        new WindowInfo("minecraft:hopper", "Hopper or minecart with hopper", 5),
        new WindowInfo("minecraft:lectern", "Lectern", 1, true),
        new WindowInfo("minecraft:loom", "Loom", 4),
        new WindowInfo("minecraft:merchant", "Villager, Wandering Trader", 3),
        new WindowInfo("minecraft:shulker_box", "Shulker box", 27),
        new WindowInfo("minecraft:smithing", "Smithing Table", 3),
        new WindowInfo("minecraft:smoker", "Smoker", 3),
        new WindowInfo("minecraft:cartography", "Cartography Table", 3),
        new WindowInfo("minecraft:stonecutter", "Stonecutter", 2),
    ];

    public WindowInfo[] GetData()
    {
        return Windows;
    }
}
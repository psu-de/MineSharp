using MineSharp.Core.Common.Blocks;

namespace MineSharp.Data.Windows;

public class WindowData
{
    private static IDictionary<string, WindowInfo> _windowMaps = new Dictionary<string, WindowInfo>();
    private static List<WindowInfo> _windows = new List<WindowInfo>();
    private static IList<BlockType> _allowedBlocksToOpen = new List<BlockType>() {
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

    static WindowData()
    {
        Register(new WindowInfo("minecraft:generic_9x1", "", 9));
        Register(new WindowInfo("minecraft:generic_9x2", "", 18));
        Register(new WindowInfo("minecraft:generic_9x3", "", 27));
        Register(new WindowInfo("minecraft:generic_9x4", "", 36));
        Register(new WindowInfo("minecraft:generic_9x5", "", 45));
        Register(new WindowInfo("minecraft:generic_9x6", "", 54));
        Register(new WindowInfo("minecraft:generic_3x3", "", 9));
        Register(new WindowInfo("minecraft:anvil", "Anvil", 3));
        Register(new WindowInfo("minecraft:beacon", "Beacon", 1));
        Register(new WindowInfo("minecraft:blast_furnace", "Blast Furnace", 3));
        Register(new WindowInfo("minecraft:brewing_stand", "Brewing stand", 5));
        Register(new WindowInfo("minecraft:crafting", "Crafting table", 10));
        Register(new WindowInfo("minecraft:enchantment", "Enchantment table", 2));
        Register(new WindowInfo("minecraft:furnace", "Furnace", 3));
        Register(new WindowInfo("minecraft:grindstone", "Grindstone", 3));
        Register(new WindowInfo("minecraft:hopper", "Hopper or minecart with hopper", 5));
        Register(new WindowInfo("minecraft:lectern", "Lectern", 1, true));
        Register(new WindowInfo("minecraft:loom", "Loom", 4));
        Register(new WindowInfo("minecraft:merchant", "Villager, Wandering Trader", 3));
        Register(new WindowInfo("minecraft:shulker_box", "Shulker box", 27));
        Register(new WindowInfo("minecraft:smithing", "Smithing Table", 3));
        Register(new WindowInfo("minecraft:smoker", "Smoker", 3));
        Register(new WindowInfo("minecraft:cartography", "Cartography Table", 3));
        Register(new WindowInfo("minecraft:stonecutter", "Stonecutter", 2));
    }


    public IList<BlockType> AllowedBlocksToOpen => _allowedBlocksToOpen;
    public List<WindowInfo> Windows => _windows;
    public IDictionary<string, WindowInfo> WindowMap => _windowMaps;

    private static void Register(WindowInfo info)
    {
        _windowMaps.Add(info.Name, info);
        _windows.Add(info);
    }
}
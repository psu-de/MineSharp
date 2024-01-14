using MineSharp.Core.Common.Blocks;

namespace MineSharp.Data.Windows;

/// <summary>
/// Static Window data
/// </summary>
public class WindowProvider
{
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
    private WindowVersion version;
    private IDictionary<string, WindowInfo> windowMap;

    internal WindowProvider(WindowVersion version)
    {
        this.version = version;
        this.windowMap = this.Windows.ToDictionary(x => x.Name);
    }


    /// <summary>
    /// All blocks that can be opened
    /// </summary>
    public IList<BlockType> AllowedBlocksToOpen => _allowedBlocksToOpen;
    
    /// <summary>
    /// All available windows
    /// </summary>
    public WindowInfo[] Windows => this.version.Windows;
    
    /// <summary>
    /// Map of window string id to WindowInfo
    /// </summary>
    public IDictionary<string, WindowInfo> WindowMap => windowMap;
}
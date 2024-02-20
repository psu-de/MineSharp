using MineSharp.Core.Common.Blocks;
using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;

namespace MineSharp.Data.Windows;

internal class WindowData(IDataProvider<WindowInfo[]> provider) : IndexedData<WindowInfo[]>(provider), IWindowData
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
    
    private IDataProvider<WindowInfo[]> provider;
    private IDictionary<string, WindowInfo> windowMap;


    /// <summary>
    /// All blocks that can be opened
    /// </summary>
    public IList<BlockType> AllowedBlocksToOpen => _allowedBlocksToOpen;

    private WindowInfo[]? Windows { get; set; }

    protected override void InitializeData(WindowInfo[] data)
    {
        this.Windows = data;
        this.windowMap = this.Windows.ToDictionary(x => x.Name);
    }

    public WindowInfo ById(int id)
    {
        if (!this.Loaded)
            this.Load();
        
        return this.Windows![id];
    }

    public WindowInfo ByName(string name)
    {
        if (!this.Loaded)
            this.Load();

        return this.windowMap[name];
    }
}
using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;

namespace MineSharp.Data.Windows;

internal class WindowData(IDataProvider<WindowInfo[]> provider) : IndexedData<WindowInfo[]>(provider), IWindowData
{
    private static readonly IList<BlockType> StaticAllowedBlocksToOpen = new List<BlockType>
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

    private Dictionary<Identifier, WindowInfo> windowMap = new();

    private WindowInfo[]? Windows { get; set; }


    /// <summary>
    ///     All blocks that can be opened
    /// </summary>
    public IList<BlockType> AllowedBlocksToOpen => StaticAllowedBlocksToOpen;

    public WindowInfo ById(int id)
    {
        if (!Loaded)
        {
            Load();
        }

        return Windows![id];
    }

    public WindowInfo ByName(Identifier name)
    {
        if (!Loaded)
        {
            Load();
        }

        return windowMap[name];
    }

    protected override void InitializeData(WindowInfo[] data)
    {
        Windows = data;
        windowMap = Windows.ToDictionary(x => x.Name);
    }
}

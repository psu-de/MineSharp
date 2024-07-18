using MineSharp.Core.Common.Blocks;
using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;

namespace MineSharp.Data.Blocks;

internal class BlockData(IDataProvider<BlockInfo[]> provider)
    : TypeIdNameIndexedData<BlockType, BlockInfo>(provider), IBlockData
{
    private BlockInfo[]? sortedByState;

    public int TotalBlockStateCount { get; private set; } = -1;

    public BlockInfo? ByState(int state)
    {
        if (!Loaded)
        {
            Load();
        }

        var half = sortedByState!.Length / 2;
        var start = state < sortedByState![half].MaxState
            ? 0
            : half;

        for (var i = start; i < sortedByState.Length; i++)
        {
            if (state <= sortedByState[i].MaxState)
            {
                return sortedByState[i];
            }
        }

        return null;
    }

    protected override void InitializeData(BlockInfo[] data)
    {
        Array.Sort(data, (a, b) => a.MinState.CompareTo(b.MinState));
        sortedByState = data;
        TotalBlockStateCount = sortedByState[^1].MaxState + 1;

        base.InitializeData(data);
    }
}

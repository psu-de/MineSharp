using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;

namespace MineSharp.Data.BlockCollisionShapes;

internal abstract class BlockCollisionShapesVersion
{
    public abstract Dictionary<BlockType, int[]> BlockToIndicesMap { get; }
    public abstract Dictionary<int, AABB[]> BlockShapes { get; }
}

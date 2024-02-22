using MineSharp.Core.Common.Blocks;

namespace MineSharp.Data.BlockCollisionShapes;

internal record BlockCollisionShapeDataBlob(
    Dictionary<BlockType, int[]> BlockToIndicesMap,
    Dictionary<int, float[][]> IndexToShapeMap);
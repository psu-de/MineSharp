using MineSharp.Core.Common.Blocks;

namespace MineSharp.Data.BlockCollisionShapes;

public record BlockCollisionShapeDataBlob(
    Dictionary<BlockType, int[]> BlockToIndicesMap,
    Dictionary<int, float[][]> IndexToShapeMap);
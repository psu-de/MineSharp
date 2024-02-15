namespace MineSharp.Data.BlockCollisionShapes;

public record BlockCollisionShapeDataBlob(
    Dictionary<string, int[]> BlockToIndicesMap,
    Dictionary<int, float[][]> IndexToShapeMap);
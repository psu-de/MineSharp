namespace MineSharp.Core.Common.Blocks;

public record BlockInfo(
    int Id,
    string Name,
    string DisplayName,
    float Hardness,
    float Resistance,
    bool Diggable,
    bool Transparent,
    int FilterLight,
    int EmitLight,
    string BoundingBox,
    int StackSize,
    string Material,
    int DefaultState,
    int MinStateId,
    int MaxStateId,
    int[]? HarvestTools,
    BlockProperties Properties,
    int[] BlockShapeIndices);
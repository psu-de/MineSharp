namespace MineSharp.Core.Common.Blocks;

public class BlockInfo
{
    public readonly int Id;
    public readonly BlockType Type;
    public readonly string Name;
    public readonly string DisplayName;
    public readonly float Hardness;
    public readonly float Resistance;
    public readonly int MinState;
    public readonly int MaxState;
    public readonly bool Diggable;
    public readonly bool Transparent;
    public readonly byte FilterLight;
    public readonly byte EmitLight;
    public readonly string BoundingBox;
    public readonly int StackSize;
    public readonly Material[] Materials;
    public readonly int[]? HarvestTools;
    public readonly int DefaultState;
    public readonly BlockState State;

    public BlockInfo(
        int id,
        BlockType type,
        string name,
        string displayName,
        float hardness,
        float resistance,
        int minState,
        int maxState,
        bool diggable,
        bool transparent,
        byte filterLight,
        byte emitLight,
        string boundingBox,
        int stackSize,
        Material[] materials,
        int[]? harvestTools,
        int defaultState,
        BlockState state)
    {
        this.Id = id;
        this.Type = type;
        this.Name = name;
        this.DisplayName = displayName;
        this.Hardness = hardness;
        this.Resistance = resistance;
        this.MinState = minState;
        this.MaxState = maxState;
        this.Diggable = diggable;
        this.Transparent = transparent;
        this.FilterLight = filterLight;
        this.EmitLight = emitLight;
        this.BoundingBox = boundingBox;
        this.StackSize = stackSize;
        this.Materials = materials;
        this.HarvestTools = harvestTools;
        this.DefaultState = defaultState;
        this.State = state;
    }

    public bool IsSolid()
        => this.Type != BlockType.Air && this.Type != BlockType.CaveAir && this.Type != BlockType.VoidAir; 
}

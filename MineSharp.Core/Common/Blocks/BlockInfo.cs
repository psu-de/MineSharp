using MineSharp.Core.Common.Items;

namespace MineSharp.Core.Common.Blocks;

/// <summary>
/// Block descriptor
/// </summary>
/// <param name="id"></param>
/// <param name="type"></param>
/// <param name="name"></param>
/// <param name="displayName"></param>
/// <param name="hardness"></param>
/// <param name="resistance"></param>
/// <param name="minState"></param>
/// <param name="maxState"></param>
/// <param name="unbreakable"></param>
/// <param name="transparent"></param>
/// <param name="filterLight"></param>
/// <param name="emitLight"></param>
/// <param name="materials"></param>
/// <param name="harvestTools"></param>
/// <param name="defaultState"></param>
/// <param name="state"></param>
public class BlockInfo(
    int        id,
    BlockType  type,
    string     name,
    string     displayName,
    float      hardness,
    float      resistance,
    int        minState,
    int        maxState,
    bool       unbreakable,
    bool       transparent,
    byte       filterLight,
    byte       emitLight,
    Material[] materials,
    ItemType[] harvestTools,
    int        defaultState,
    BlockState state)
{
    /// <summary>
    /// The numerical id of this block (depends on Minecraft version)
    /// </summary>
    public int Id { get; } = id;

    /// <summary>
    /// The internal <see cref="BlockType"/> of this block (independent from Minecraft version)
    /// </summary>
    public BlockType Type { get; } = type;

    /// <summary>
    /// The text id of this block
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Minecraft's display name for this block
    /// </summary>
    public string DisplayName { get; } = displayName;

    /// <summary>
    /// Hardness value of the block
    /// </summary>
    public float Hardness { get; } = hardness;

    /// <summary>
    /// Resistance value of this block
    /// </summary>
    public float Resistance { get; } = resistance;

    /// <summary>
    /// The smallest block state possible.
    /// </summary>
    public int MinState { get; } = minState;

    /// <summary>
    /// The highest block state possible.
    /// </summary>
    public int MaxState { get; } = maxState;

    /// <summary>
    /// Whether this block is unbreakable
    /// </summary>
    public bool Unbreakable { get; } = unbreakable;

    /// <summary>
    /// Whether this block is transparent
    /// </summary>
    public bool Transparent { get; } = transparent;

    /// <summary>
    /// How much light this block filters (0-15)
    /// </summary>
    public byte FilterLight { get; } = filterLight;

    /// <summary>
    /// How much light this block is emitting (0-15)
    /// </summary>
    public byte EmitLight { get; } = emitLight;

    /// <summary>
    /// An array of Materials which can be used to destroy this block faster
    /// </summary>
    public Material[] Materials { get; } = materials;

    /// <summary>
    /// An array of Items that can be used to harvest this block
    /// </summary>
    public ItemType[] HarvestTools { get; } = harvestTools;

    /// <summary>
    /// The default state of this block
    /// </summary>
    public int DefaultState { get; } = defaultState;

    /// <summary>
    /// The block state containing all properties of this block
    /// </summary>
    public BlockState State { get; } = state;

    /// <summary>
    /// Whether this block is considered solid.
    /// TODO: Minecraft calculates this based on block collision shapes.
    /// BlockBehaviour.java:470 calculateSolid()
    /// </summary>
    /// <returns></returns>
    public bool IsSolid()
        => this.Type != BlockType.Air && this.Type != BlockType.CaveAir && this.Type != BlockType.VoidAir;

    /// <summary>
    /// Whether this block is a fluid.
    /// </summary>
    /// <returns></returns>
    public bool IsFluid()
        => this.Type is BlockType.Water or BlockType.Lava;
}

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
/// <param name="boundingBox"></param>
/// <param name="stackSize"></param>
/// <param name="materials"></param>
/// <param name="harvestTools"></param>
/// <param name="defaultState"></param>
/// <param name="state"></param>
public class BlockInfo(
    int id,
    BlockType type,
    string name,
    string displayName,
    float hardness,
    float resistance,
    int minState,
    int maxState,
    bool unbreakable,
    bool transparent,
    byte filterLight,
    byte emitLight,
    Material[] materials,
    ItemType[] harvestTools,
    int defaultState,
    BlockState state)
{
    /// <summary>
    /// The numerical id of this block (depends on Minecraft version)
    /// </summary>
    public readonly int Id = id;
    /// <summary>
    /// The internal <see cref="BlockType"/> of this block (independent from Minecraft version)
    /// </summary>
    public readonly BlockType Type = type;
    /// <summary>
    /// The text id of this block
    /// </summary>
    public readonly string Name = name;
    /// <summary>
    /// Minecraft's display name for this block
    /// </summary>
    public readonly string DisplayName = displayName;
    /// <summary>
    /// Hardness value of the block
    /// </summary>
    public readonly float Hardness = hardness;
    /// <summary>
    /// Resistance value of this block
    /// </summary>
    public readonly float Resistance = resistance;
    /// <summary>
    /// The smallest block state possible.
    /// </summary>
    public readonly int MinState = minState;
    /// <summary>
    /// The highest block state possible.
    /// </summary>
    public readonly int MaxState = maxState;
    /// <summary>
    /// Whether this block is unbreakable
    /// </summary>
    public readonly bool Unbreakable = unbreakable;
    /// <summary>
    /// Whether this block is transparent
    /// </summary>
    public readonly bool Transparent = transparent;
    /// <summary>
    /// How much light this block filters (0-15)
    /// </summary>
    public readonly byte FilterLight = filterLight;
    /// <summary>
    /// How much light this block is emitting (0-15)
    /// </summary>
    public readonly byte EmitLight = emitLight;
    /// <summary>
    /// An array of Materials which can be used to destroy this block faster
    /// </summary>
    public readonly Material[] Materials = materials;
    /// <summary>
    /// An array of Items that can be used to harvest this block
    /// </summary>
    public readonly ItemType[] HarvestTools = harvestTools;
    /// <summary>
    /// The default state of this block
    /// </summary>
    public readonly int DefaultState = defaultState;
    /// <summary>
    /// The block state containing all properties of this block
    /// </summary>
    public readonly BlockState State = state;

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

using MineSharp.Core.Common.Items;

namespace MineSharp.Core.Common.Blocks;

/// <summary>
/// A minecraft block
/// </summary>
/// <param name="info"></param>
/// <param name="state"></param>
/// <param name="position"></param>
public class Block(BlockInfo info, int state, Position position)
{
    /// <summary>
    /// Descriptor of this block
    /// </summary>
    public BlockInfo Info { get; } = info;

    /// <summary>
    /// The block state of this block
    /// </summary>
    public int State { get; set; } = state;

    /// <summary>
    /// Position of this block
    /// </summary>
    public Position Position { get; set; } = position;

    /// <summary>
    /// Metadata value of this block
    /// </summary>
    public int Metadata => this.State - this.Info.MinState;

    /// <summary>
    /// Get a property of this block. 
    /// </summary>
    /// <param name="name">The name of the property</param>
    /// <typeparam name="T">Must be string, int or bool. Depending on the type of the property.</typeparam>
    /// <returns></returns>
    public T GetProperty<T>(string name)
        => this.Info.State.GetPropertyValue<T>(name, this.State);


    /// <summary>
    /// Whether this block is considered solid
    /// </summary>
    /// <returns></returns>
    public bool IsSolid()
        => this.Info.IsSolid();

    /// <summary>
    /// Whether this block is considered a fluid
    /// </summary>
    /// <returns></returns>
    public bool IsFluid()
        => this.Info.IsFluid();

    /// <summary>
    /// Whether this block can be harvested by <paramref name="item"/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool CanBeHarvestedBy(ItemType? item)
        => this.Info.HarvestTools == null 
           || (item != null && this.Info.HarvestTools.Contains(item.Value));

    /// <inheritdoc />
    public override string ToString() => $"Block (Position={Position}, State={State}, Info={Info})";
}

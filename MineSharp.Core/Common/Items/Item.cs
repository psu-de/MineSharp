using fNbt;

namespace MineSharp.Core.Common.Items;

/// <summary>
/// Represents an Item
/// </summary>
/// <param name="info"></param>
/// <param name="count"></param>
/// <param name="damage"></param>
/// <param name="metadata"></param>
public class Item(ItemInfo info, byte count, int? damage, NbtCompound? metadata)
{
    /// <summary>
    /// The item descriptor
    /// </summary>
    public readonly ItemInfo Info = info;

    /// <summary>
    /// The number of items on the stack
    /// </summary>
    public byte Count { get; set; } = count;

    /// <summary>
    /// Optional value for the durability of a tool
    /// </summary>
    public int? Damage { get; set; } = damage;

    /// <summary>
    /// Additional metadata of this item
    /// </summary>
    public NbtCompound? Metadata { get; set; } = metadata;

    /// <summary>
    /// Returns a clone of this Item
    /// </summary>
    /// <returns></returns>
    public Item Clone()
    {
        return new Item(this.Info, Count, Damage, Metadata);
    }

    /// <inheritdoc />
    public override string ToString()
        => $"Item(Type={Info.Type}, Count={Count}, Damage={Damage}, Metadata={Metadata})";
}

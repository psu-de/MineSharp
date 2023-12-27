using MineSharp.Core.Common.Items;

namespace MineSharp.Core.Common;

/// <summary>
/// Represents a minecraft slot with an item and slot index
/// </summary>
/// <param name="item"></param>
/// <param name="slotIndex"></param>
public class Slot(Item? item, short slotIndex)
{
    /// <summary>
    /// The item of this Slot. Null when slot is empty
    /// </summary>
    public Item? Item { get; set; } = item;
    
    /// <summary>
    /// The index of this slot
    /// </summary>
    public short SlotIndex { get; set; } = slotIndex;

    /// <summary>
    /// Whether this slot is empty
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty() => this.Item == null || this.Item.Count <= 0;
    
    /// <summary>
    /// Whether this slot is full (item's stack size is reached)
    /// </summary>
    /// <returns></returns>
    public bool IsFull() => this.Item != null && this.Item.Count == this.Item.Info.StackSize;
    
    /// <summary>
    /// How many items can be stacked on this slot
    /// </summary>
    public int LeftToStack => (this.Item?.Info.StackSize - this.Item?.Count) ?? throw new NotSupportedException();

    /// <summary>
    /// Whether <paramref name="count"/> of the other slot's item can be stacked on this slot's item
    /// </summary>
    /// <param name="otherSlot"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public bool CanStack(Slot otherSlot, int count)
    {
        return this.CanStack(otherSlot.Item?.Info.Type, count);
    }
        
    /// <summary>
    /// Whether all of <paramref name="otherSlot"/>'s items can be stacked on this slot's item
    /// </summary>
    /// <param name="otherSlot"></param>
    /// <returns></returns>
    public bool CanStack(Slot otherSlot)
    {
        return this.CanStack(otherSlot.Item?.Info.Type, otherSlot.Item?.Count);
    }

    /// <summary>
    /// Whether <paramref name="count"/> items of type <paramref name="type"/> can be stacked on this slot's item
    /// </summary>
    /// <param name="type"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public bool CanStack(ItemType? type, int? count = null)
    {
        count ??= 1;
        if (this.IsEmpty() || type == null)
        {
            return true;
        }
            
        var slotType = this.Item!.Info.Type;

        if (slotType == type)
        {

            if (this.Item!.Info.StackSize == 1) return false;

            return this.LeftToStack >= count;

        }
        return false;
    }

    /// <inheritdoc />
    public override string ToString() => $"Slot (Index={this.SlotIndex} Item={this.Item?.ToString() ?? "None"})";
}

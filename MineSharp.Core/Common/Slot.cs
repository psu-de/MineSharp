using MineSharp.Core.Common.Items;

namespace MineSharp.Core.Common;

/// <summary>
///     Represents a minecraft slot with an item and slot index
/// </summary>
/// <param name="item"></param>
/// <param name="slotIndex"></param>
public class Slot(Item? item, short slotIndex)
{
    /// <summary>
    ///     The item of this Slot. Null when slot is empty
    /// </summary>
    public Item? Item { get; set; } = item;

    /// <summary>
    ///     The index of this slot
    /// </summary>
    public short SlotIndex { get; set; } = slotIndex;

    /// <summary>
    ///     How many items can be stacked on this slot
    /// </summary>
    public int LeftToStack => Item?.Info.StackSize - Item?.Count ?? throw new NotSupportedException();

    /// <summary>
    ///     Whether this slot is empty
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        return Item == null || Item.Count <= 0;
    }

    /// <summary>
    ///     Whether this slot is full (item's stack size is reached)
    /// </summary>
    /// <returns></returns>
    public bool IsFull()
    {
        return Item != null && Item.Count == Item.Info.StackSize;
    }

    /// <summary>
    ///     Whether <paramref name="count" /> of the other slot's item can be stacked on this slot's item
    /// </summary>
    /// <param name="otherSlot"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public bool CanStack(Slot otherSlot, int count)
    {
        return CanStack(otherSlot.Item?.Info.Type, count);
    }

    /// <summary>
    ///     Whether all of <paramref name="otherSlot" />'s items can be stacked on this slot's item
    /// </summary>
    /// <param name="otherSlot"></param>
    /// <returns></returns>
    public bool CanStack(Slot otherSlot)
    {
        return CanStack(otherSlot.Item?.Info.Type, otherSlot.Item?.Count);
    }

    /// <summary>
    ///     Whether <paramref name="count" /> items of type <paramref name="type" /> can be stacked on this slot's item
    /// </summary>
    /// <param name="type"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public bool CanStack(ItemType? type, int? count = null)
    {
        count ??= 1;
        if (IsEmpty() || type == null)
        {
            return true;
        }

        var slotType = Item!.Info.Type;

        if (slotType == type)
        {
            if (Item!.Info.StackSize == 1)
            {
                return false;
            }

            return LeftToStack >= count;
        }

        return false;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Slot (Index={SlotIndex} Item={Item?.ToString() ?? "None"})";
    }
}

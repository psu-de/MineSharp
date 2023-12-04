using MineSharp.Core.Common.Items;

namespace MineSharp.Core.Common;

public class Slot
{
    public Item? Item { get; set; }
    public short SlotIndex { get; set; }
    
    public Slot(Item? item, short slotIndex)
    {
        this.Item = item;
        this.SlotIndex = slotIndex;
    }
    
    public bool IsEmpty() => this.Item == null || this.Item.Count <= 0;
    public bool IsFull() => this.Item != null && this.Item.Count == this.Item.Info.StackSize;
    
    /// <summary>
    /// How many items can be stacked on this slot
    /// </summary>
    public int LeftToStack => (this.Item?.Info.StackSize - this.Item?.Count) ?? throw new NotSupportedException();

    public bool CanStack(Slot otherSlot, int count)
    {
        return this.CanStack(otherSlot.Item?.Info.Id, count);
    }
        
    public bool CanStack(Slot otherSlot)
    {
        return this.CanStack(otherSlot.Item?.Info.Id, otherSlot.Item?.Count);
    }

    public bool CanStack(int? itemId, int? count = null)
    {
        count ??= 1;
        if (this.IsEmpty() || itemId == null)
        {
            return true;
        }
            
        var slotType = this.Item!.Info.Id;

        if (slotType == itemId)
        {

            if (this.Item!.Info.StackSize == 1) return false;

            return this.LeftToStack >= count;

        }
        return false;
    }

    public override string ToString() => $"Slot (Index={this.SlotIndex} Item={this.Item?.ToString() ?? "None"})";
}

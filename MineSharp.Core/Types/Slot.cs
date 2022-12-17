namespace MineSharp.Core.Types
{
    public class Slot
    {

        public Slot(Item? item, short slotNumber)
        {
            this.Item = item;
            this.SlotNumber = slotNumber;
        }

        public Item? Item { get; set; }
        public short SlotNumber { get; set; }

        public bool IsEmpty() => this.Item == null;
        public bool IsFull() => this.Item != null && this.Item.Count == this.Item.StackSize;
        
        /// <summary>
        /// How many items can be stacked on this slot
        /// </summary>
        public int LeftToStack => (this.Item?.StackSize - this.Item?.Count) ?? throw new NotSupportedException();

        public bool CanStack(Slot otherSlot, int count)
        {
            return this.CanStack(otherSlot.Item?.Id, count);
        }
        
        public bool CanStack(Slot otherSlot)
        {
            return this.CanStack(otherSlot.Item?.Id, otherSlot.Item?.Count);
        }

        public bool CanStack(int? itemId, int? count = null)
        {
            count ??= 1;
            if (this.IsEmpty() || itemId == null)
            {
                return true;
            }
            
            var slotType = this.Item!.Id;

            if (slotType == itemId)
            {

                if (this.Item!.StackSize == 1) return false;

                return this.LeftToStack >= count;

            }
            return false;
        }

        public Slot Clone() => new Slot(this.Item, this.SlotNumber);

        public override string ToString() => $"Slot (Index={this.SlotNumber} Item={this.Item?.ToString() ?? "None"})";
    }
}

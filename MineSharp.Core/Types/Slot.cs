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

        public bool CanStack(Slot otherSlot)
        {
            return this.CanStack(otherSlot.Item?.Id);
        }

        public bool CanStack(int? itemId)
        {
            if (this.IsEmpty() || itemId == null)
            {
                return true;
            }
            
            var slotType = this.Item!.Id;

            if (slotType == itemId)
            {

                if (this.Item!.StackSize == 1) return false;

                return this.Item!.StackSize - this.Item.Count > 0;

            }
            return false;
        }

        public Slot Clone() => new Slot(this.Item, this.SlotNumber);

        public override string ToString() => $"Slot (Index={this.SlotNumber} Item={this.Item?.ToString() ?? "None"})";
    }
}

namespace MineSharp.Core.Types {
    public partial class Slot {

        public Item? Item { get; set; } = null;
        public short SlotNumber { get; set; }

        public Slot(Item? item, short slotNumber) {
            this.Item = item;
            this.SlotNumber = slotNumber;
        }

        public bool IsEmpty() => Item == null;

        public bool CanStack(Slot otherSlot) {
            if (this.IsEmpty() || otherSlot.IsEmpty()) return true;

            var slotType = this.Item!.Id;
            var otherSlotType = otherSlot.Item!.Id;

            if (slotType == otherSlotType) {

                if (this.Item!.StackSize == 1) return false;

                return (this.Item!.StackSize - this.Item.Count) > 0;

            } else return false;
        }

        public Slot Clone() {
            return new Slot(this.Item, this.SlotNumber);
        }

        public override string ToString()
        {
            return $"Slot (Index={SlotNumber} Item={Item?.ToString() ?? "None"})";
        }
    }
}

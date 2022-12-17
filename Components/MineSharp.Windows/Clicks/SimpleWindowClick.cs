using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;

namespace MineSharp.Windows.Clicks
{
    internal class SimpleWindowClick : WindowClick
    {
        public override WindowOperationMode ClickMode => WindowOperationMode.SimpleClick;

        internal SimpleWindowClick(Window window, short slot, byte button) : base(window, slot, button)
        { }
        
        private void PerformOutsideClick()
        {
            // Clicked outside, drop item stack
            if (this.Window.GetSelectedSlot().IsEmpty())
            {
                return;
            }

            var selectedItem = this.Window.GetSelectedSlot();
            if (this.Button == 0) // Drop entire stack
            {
                selectedItem.Item = null;
            } else
            { // Drop one at a time
                selectedItem.Item!.Count--;
                if (selectedItem.Item!.Count == 0)
                {
                    selectedItem.Item = null;
                }
            }
            this.Window.SetSelectedSlot(selectedItem);
        }

        private void PerformLeftClick()
        {
            // Swap selected slot and clicked slot
            var temp = this.Window.GetSelectedSlot().Clone();
            var clickedSlot = this.Window.GetSlot(this.Slot);

            clickedSlot.SlotNumber = -1;
            this.Window.SetSlot(clickedSlot); // set selected slot

            temp.SlotNumber = this.Slot;
            this.Window.SetSlot(temp);
        }

        private void PerformRightClick()
        {
            if (this.Window.GetSelectedSlot().IsEmpty() && this.Window.GetSlot(this.Slot).IsEmpty())
            {
                return;
            }

            if (this.Window.GetSelectedSlot().IsEmpty())
            { // Pickup half stack
                var oldSlot = this.Window.GetSlot(this.Slot);
                var count = (byte)Math.Ceiling(oldSlot.Item!.Count / 2.0F);
                var newSelected = new Slot(oldSlot.Item, -1); // Clone Item?
                this.Window.SetSlot(newSelected);
                
                oldSlot.Item.Count -= count;
                this.Window.SetSlot(oldSlot);
                return;
            }

            if (this.Window.GetSlot(this.Slot).IsEmpty() || this.Window.GetSlot(this.Slot).CanStack(this.Window.GetSelectedSlot()))
            { // Transfer one item from selectedSlot to slots[Slot]
                var oldSlot = this.Window.GetSelectedSlot().Clone();
                this.Window.SetSlot(new Slot(oldSlot.Item, this.Slot)); // Clone Item?
                oldSlot.Item!.Count--;
                if (oldSlot.Item!.Count == 0) oldSlot.Item = null;
                oldSlot.SlotNumber = -1;
                this.Window.SetSlot(oldSlot);
            } else
            {
                // just swap selected slot and clicked swap, like a left click
                this.PerformLeftClick();
            }
        }

        public override void PerformClick()
        {
            if (!(this.Button == 0 || this.Button == 1))
                throw new NotSupportedException();

            if (this.Slot == OutsideClick)
            {
                this.PerformOutsideClick();
                return;
            }

            if (this.Button == 0)
            { // Swap selected slot and clicked slot
                this.PerformLeftClick();
                return;
            } else
            {
                this.PerformRightClick();
            }
        }
    }
}

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
            var selectedSlot = this.Window.GetSelectedSlot().Clone();
            var clickedSlot = this.Window.GetSlot(this.Slot);

            if (selectedSlot.IsEmpty() && clickedSlot.IsEmpty())
            {
                return;
            }

            if (selectedSlot.Item?.Id == clickedSlot.Item?.Id)
            {
                // stack items, both items cannot be null
                int left = selectedSlot.Item!.Count - clickedSlot.LeftToStack;
                if (left < 0)
                {
                    left = 0;
                }
                clickedSlot.Item!.Count += (byte)(selectedSlot.Item!.Count - left);
                selectedSlot.Item!.Count = (byte)left;
                this.Window.SetSlot(clickedSlot);
                this.Window.SetSlot(selectedSlot);
                return;
            }
            
            //swap items
            clickedSlot.SlotNumber = -1;
            this.Window.SetSlot(clickedSlot); // set selected slot

            selectedSlot.SlotNumber = this.Slot;
            this.Window.SetSlot(selectedSlot);
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
                var selectedSlot = this.Window.GetSelectedSlot();
                selectedSlot.Item = oldSlot.Item.Clone();
                selectedSlot.Item.Count = count;

                oldSlot.Item.Count -= count;
                
                this.Window.SetSlot(oldSlot);
                return;
            }

            if (this.Window.GetSlot(this.Slot).IsEmpty() || this.Window.GetSlot(this.Slot).CanStack(this.Window.GetSelectedSlot()))
            { // Transfer one item from selectedSlot to slots[Slot]
                var selectedSlot = this.Window.GetSelectedSlot();
                selectedSlot.Item!.Count -= 1;

                var clickedSlot = this.Window.GetSlot(this.Slot);

                if (clickedSlot.IsEmpty())
                {
                    clickedSlot.Item = selectedSlot.Item!.Clone();
                    clickedSlot.Item!.Count = 1;
                } else
                {
                    clickedSlot.Item!.Count += 1;
                }
                    
                this.Window.SetSelectedSlot(selectedSlot);
                this.Window.SetSlot(clickedSlot); // Clone Item?
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

using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;

namespace MineSharp.Windows.Old
{
    public class WindowClick
    {

        public const int OutsideSlot = -999;

        public WindowClick(WindowOperationMode clickMode, byte button, short slot)
        {
            this.ClickMode = clickMode;
            this.Button = button;
            this.Slot = slot;
        }


        public WindowOperationMode ClickMode { get; set; }
        public byte Button { get; set; }
        public short Slot { get; set; }

        internal void PerformClick(Window window)
        {


            //TODO: Implement all WindowOperationModes
            switch (this.ClickMode)
            {
                case WindowOperationMode.SimpleClick:
                    this.PerformSimpleClick(window);
                    break;
                default:
                    throw new NotImplementedException();
            }

        }


        private void PerformSimpleClick(Window window)
        {
            Logger.GetLogger().Debug(window.GetSlot(Slot).ToString());
            if (!(this.Button == 0 || this.Button == 1))
                throw new NotSupportedException();

            if (this.Slot == OutsideSlot)
            { // Clicked outside, drop item stack
                if (window.SelectedSlot!.IsEmpty()) return;

                if (this.Button == 0) // Drop entire stack
                    window.SelectedSlot.Item = null;
                else
                { // Drop one at a time
                    window.SelectedSlot.Item!.Count--;
                    if (window.SelectedSlot.Item!.Count == 0)
                        window.SelectedSlot.Item = null;
                }
                return;
            }

            if (this.Button == 0)
            { // Swap selected slot and clicked slot
                window.SwapSelectedSlot(this.Slot);
                return;
            }
            if (window.SelectedSlot!.IsEmpty() && window.GetSlot(this.Slot).IsEmpty())
                return;

            if (window.SelectedSlot.IsEmpty())
            { // Pickup half stack
                var oldSlot = window.GetSlot(this.Slot);
                var count = (byte)Math.Ceiling(oldSlot.Item!.Count / 2.0F);
                window.SelectedSlot = new Slot(oldSlot.Item, -1); // Clone Item?
                oldSlot.Item.Count -= count;
                window.SetSlot(oldSlot);
                return;
            }

            if (window.GetSlot(this.Slot).IsEmpty() || window.GetSlot(this.Slot).CanStack(window.SelectedSlot!))
            { // Transfer one item from selectedSlot to slots[Slot]
                var oldSlot = window.SelectedSlot;
                window.SetSlot(new Slot(oldSlot.Item, this.Slot)); // Clone Item?
                oldSlot.Item!.Count--;
                if (oldSlot.Item!.Count == 0) oldSlot.Item = null;
                oldSlot.SlotNumber = -1;
                window.SelectedSlot = oldSlot;
            } else
            {
                window.SwapSelectedSlot(this.Slot);
            }
        }

        public override string ToString() => $"Window (Type={ClickMode}, Button={Button}, Slot={Slot})";
    }
}

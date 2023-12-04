using MineSharp.Core.Common;
using MineSharp.Core.Common.Items;

namespace MineSharp.Windows.Clicks;

internal class SimpleWindowClick : WindowClick
{
    public override ClickMode ClickMode => ClickMode.SimpleClick;

    private IList<Slot> _changedSlots = new List<Slot>();

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
        
        this.Window.UpdateSlot(selectedItem.Item, Window.SELECTED_SLOT);
    }

    private void PerformLeftClick()
    {
        // Swap selected slot and clicked slot
        var selectedSlot = this.Window.GetSelectedSlot();
        var clickedSlot = this.Window.GetSlot(this.Slot);

        if (selectedSlot.IsEmpty() && clickedSlot.IsEmpty())
            return;

        this._changedSlots.Add(clickedSlot);

        if (selectedSlot.Item?.Info.Id == clickedSlot.Item?.Info.Id)
        {
            // stack items, both items cannot be null
            int left = selectedSlot.Item!.Count - clickedSlot.LeftToStack;
            if (left < 0)
                left = 0;
            
            clickedSlot.Item!.Count += (byte)(selectedSlot.Item!.Count - left);
            selectedSlot.Item!.Count = (byte)left;

            if (selectedSlot.Item!.Count == 0) 
                selectedSlot.Item = null;
            
            this.Window.UpdateSlot(clickedSlot.Item, this.Slot);
            this.Window.UpdateSlot(selectedSlot.Item, Window.SELECTED_SLOT);
            return;
        }
        
        //swap items
        (clickedSlot.Item, selectedSlot.Item) = (selectedSlot.Item, clickedSlot.Item);
        this.Window.UpdateSlot(clickedSlot.Item, this.Slot);
        this.Window.UpdateSlot(selectedSlot.Item, Window.SELECTED_SLOT);
    }

    private void PerformRightClick()
    {
        var clickedSlot = this.Window.GetSlot(this.Slot);
        var selectedSlot = this.Window.GetSelectedSlot();
        if (selectedSlot.IsEmpty() && clickedSlot.IsEmpty())
            return;
        
        if (selectedSlot.IsEmpty())
        { 
            // Pickup half stack
            var count = (byte)Math.Ceiling(clickedSlot.Item!.Count / 2.0F);
            var newSelectedItem = clickedSlot.Item.Clone();
            newSelectedItem.Count = count;

            clickedSlot.Item.Count -= count;
            this._changedSlots.Add(clickedSlot);
            
            this.Window.UpdateSlot(newSelectedItem, Window.SELECTED_SLOT);
            this.Window.UpdateSlot(clickedSlot.Item, clickedSlot.SlotIndex);
            return;
        }

        if (clickedSlot.IsEmpty() || clickedSlot.CanStack(selectedSlot, 1))
        { 
            // Transfer one item from selectedSlot to slots[Slot]
            selectedSlot.Item!.Count -= 1;

            if (clickedSlot.IsEmpty())
            {
                clickedSlot.Item = new Item(
                    selectedSlot.Item.Info, 
                    1, 
                    selectedSlot.Item.Damage, 
                    selectedSlot.Item.Metadata); // TODO: Clone metadata?
            } else
            {
                clickedSlot.Item!.Count += 1;
            }
            this.Window.UpdateSlot(selectedSlot.Item, Window.SELECTED_SLOT);
            this.Window.UpdateSlot(clickedSlot.Item, this.Slot);
            this._changedSlots.Add(clickedSlot);
        } else
        {
            // just swap selected slot and clicked swap, like a left click
            this.PerformLeftClick();
        }
    }

    public override void PerformClick()
    {
        this._changedSlots.Clear();
        if (!(this.Button == 0 || this.Button == 1))
            throw new NotSupportedException();

        if (this.Slot == OutsideClick)
        {
            this.PerformOutsideClick();
            return;
        }

        if (this.Button == 0)
        { 
            // Swap selected slot and clicked slot
            this.PerformLeftClick();
            return;
        }
        
        this.PerformRightClick();
    }

    public override Slot[] GetChangedSlots()
    {
        return this._changedSlots.ToArray();
    }
}
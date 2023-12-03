using MineSharp.Core.Common;

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
            
            return;
        }
        
        //swap items
        var temp = clickedSlot.Item;
        clickedSlot.Item = selectedSlot.Item;
        selectedSlot.Item = temp;
    }

    private void PerformRightClick()
    {
        Console.WriteLine("Simple right click");
        if (this.Window.GetSelectedSlot().IsEmpty() && this.Window.GetSlot(this.Slot).IsEmpty())
            return;

        var clickedSlot = this.Window.GetSlot(this.Slot);
        var selectedSlot = this.Window.GetSelectedSlot();

        if (this.Window.GetSelectedSlot().IsEmpty())
        { 
            Console.WriteLine("Simple right click pickup half");
            // Pickup half stack
            var count = (byte)Math.Ceiling(clickedSlot.Item!.Count / 2.0F);
            selectedSlot.Item = clickedSlot.Item.Clone();
            selectedSlot.Item.Count = count;

            clickedSlot.Item.Count -= count;
            this._changedSlots.Add(clickedSlot);
            return;
        }

        if (clickedSlot.IsEmpty() || clickedSlot.CanStack(selectedSlot, 1))
        { 
            Console.WriteLine("Simple right click transfer one");
            // Transfer one item from selectedSlot to slots[Slot]
            selectedSlot.Item!.Count -= 1;

            Console.WriteLine("Initial: " + clickedSlot);
            if (clickedSlot.IsEmpty())
            {
                Console.WriteLine("Empty");
                clickedSlot.Item = new Core.Common.Items.Item(
                    selectedSlot.Item.Info, 
                    1, 
                    selectedSlot.Item.Damage, 
                    selectedSlot.Item.Metadata); // TODO: Clone metadata?
                    
                Console.WriteLine(clickedSlot.Item);
            } else
            {
                clickedSlot.Item!.Count += 1;
            }
            this._changedSlots.Add(clickedSlot);
        } else
        {
            Console.WriteLine("Simple right click swap");
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
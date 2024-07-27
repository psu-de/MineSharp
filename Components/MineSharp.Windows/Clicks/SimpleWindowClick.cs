using MineSharp.Core.Common;

namespace MineSharp.Windows.Clicks;

internal class SimpleWindowClick : WindowClick
{
    private readonly IList<Slot> changedSlots = new List<Slot>();

    internal SimpleWindowClick(Window window, short slot, byte button) : base(window, slot, button)
    { }

    public override ClickMode ClickMode => ClickMode.SimpleClick;

    private void PerformOutsideClick()
    {
        // Clicked outside, drop item stack
        if (Window.GetSelectedSlot().IsEmpty())
        {
            return;
        }

        var selectedItem = Window.GetSelectedSlot();
        if (Button == 0) // Drop entire stack
        {
            selectedItem.Item = null;
        }
        else
        {
            // Drop one at a time
            selectedItem.Item!.Count--;
            if (selectedItem.Item!.Count == 0)
            {
                selectedItem.Item = null;
            }
        }

        Window.UpdateSlot(selectedItem.Item, Window.SelectedSlot);
    }

    private void PerformLeftClick()
    {
        // Swap selected slot and clicked slot
        var selectedSlot = Window.GetSelectedSlot();
        var clickedSlot = Window.GetSlot(Slot);

        if (selectedSlot.IsEmpty() && clickedSlot.IsEmpty())
        {
            return;
        }

        changedSlots.Add(clickedSlot);

        if (selectedSlot.Item?.Info.Id == clickedSlot.Item?.Info.Id)
        {
            // stack items, both items cannot be null
            var left = selectedSlot.Item!.Count - clickedSlot.LeftToStack;
            if (left < 0)
            {
                left = 0;
            }

            clickedSlot.Item!.Count += (byte)(selectedSlot.Item!.Count - left);
            selectedSlot.Item!.Count = (byte)left;

            if (selectedSlot.Item!.Count == 0)
            {
                selectedSlot.Item = null;
            }

            Window.UpdateSlot(clickedSlot.Item, Slot);
            Window.UpdateSlot(selectedSlot.Item, Window.SelectedSlot);
            return;
        }

        //swap items
        (clickedSlot.Item, selectedSlot.Item) = (selectedSlot.Item, clickedSlot.Item);
        Window.UpdateSlot(clickedSlot.Item, Slot);
        Window.UpdateSlot(selectedSlot.Item, Window.SelectedSlot);
    }

    private void PerformRightClick()
    {
        var clickedSlot = Window.GetSlot(Slot);
        var selectedSlot = Window.GetSelectedSlot();
        if (selectedSlot.IsEmpty() && clickedSlot.IsEmpty())
        {
            return;
        }

        if (selectedSlot.IsEmpty())
        {
            // Pickup half stack
            var count = (byte)Math.Ceiling(clickedSlot.Item!.Count / 2.0F);
            var newSelectedItem = clickedSlot.Item.Clone();
            newSelectedItem.Count = count;

            clickedSlot.Item.Count -= count;
            changedSlots.Add(clickedSlot);

            Window.UpdateSlot(newSelectedItem, Window.SelectedSlot);
            Window.UpdateSlot(clickedSlot.Item, clickedSlot.SlotIndex);
            return;
        }

        if (clickedSlot.IsEmpty() || clickedSlot.CanStack(selectedSlot, 1))
        {
            // Transfer one item from selectedSlot to slots[Slot]
            selectedSlot.Item!.Count -= 1;

            if (clickedSlot.IsEmpty())
            {
                clickedSlot.Item = new(
                    selectedSlot.Item.Info,
                    1,
                    selectedSlot.Item.Damage,
                    selectedSlot.Item.Metadata); // TODO: Clone metadata?
            }
            else
            {
                clickedSlot.Item!.Count += 1;
            }

            Window.UpdateSlot(selectedSlot.Item, Window.SelectedSlot);
            Window.UpdateSlot(clickedSlot.Item, Slot);
            changedSlots.Add(clickedSlot);
        }
        else
        {
            // just swap selected slot and clicked swap, like a left click
            PerformLeftClick();
        }
    }

    public override void PerformClick()
    {
        changedSlots.Clear();
        if (!(Button == 0 || Button == 1))
        {
            throw new NotSupportedException();
        }

        if (Slot == OutsideClick)
        {
            PerformOutsideClick();
            return;
        }

        if (Button == 0)
        {
            // Swap selected slot and clicked slot
            PerformLeftClick();
            return;
        }

        PerformRightClick();
    }

    public override Slot[] GetChangedSlots()
    {
        return changedSlots.ToArray();
    }
}

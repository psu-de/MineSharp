using MineSharp.Core.Common;
using MineSharp.Windows.Clicks;
using NLog;

namespace MineSharp.Windows;

public class Window
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    
    public delegate void SlotChanged(Window window, short index);
    public delegate Task WindowSynchronizer(Window window, WindowClick click);
    
    
    public byte WindowId { get; }
    public int StateId { get; set; }
    public string Title { get; }
    public Slot[] Slots { get; }
    public Window? Inventory { get; }
    public int TotalSlotCount {
        get {
            int count = this.Slots.Length;
            if (this.Inventory != null)
            {
                count += this.Inventory.Slots.Length;
            }
            if (this.OffhandSlot != null)
            {
                count += 1;
            }
            return count;
        }
    }
    
    private Slot? _selectedSlot { get; set; }
    private Slot? OffhandSlot { get; set; }

    private WindowSynchronizer? Synchronizer { get; }
    private object _syncLock { get; }
    private bool IsSynchronized => this.Synchronizer != null;
    
    public event SlotChanged OnSlotChanged;
    
    public Window(byte id, string title, int uniqueSlotCount, Window? inventory = null, WindowSynchronizer? windowSynchronizer = null)
    {
        this.WindowId = id;
        this.Title = title;
        this.Slots = new Slot[uniqueSlotCount];
        this.Inventory = inventory;
        if (inventory == null)
        {
            this._selectedSlot = new Slot(null, -1);
        }
        if (id == 0)
        {
            this.OffhandSlot = new Slot(null, 45);
        }
        
        this.Synchronizer = windowSynchronizer;
        this._syncLock = new object();

        for (short i = 0; i < uniqueSlotCount; i++)
        {
            this.Slots[i] = new Slot(null, i);
        }
    }

    #region Get / Set slots
    

    /// <summary>
    /// Sets the selected slot
    /// </summary>
    /// <param name="slot"></param>
    internal void SetSelectedSlot(Slot slot)
    {
        if (this.Inventory != null)
        {
            this.Inventory.SetSelectedSlot(slot);
        } else
        {
            slot.SlotIndex = -1;
            this._selectedSlot = slot;
        }
    }

    /// <summary>
    /// Returns the selected slot
    /// </summary>
    /// <returns></returns>
    public Slot GetSelectedSlot()
    {
        if (this.Inventory != null)
        {
            return this.Inventory.GetSelectedSlot();
        }
        return this._selectedSlot!;
    }

    /// <summary>
    /// Returns only the container slots of this window
    /// </summary>
    /// <returns></returns>
    public Slot[] GetContainerSlots()
    {
        var slots = this.Slots.Select(x => x.Clone());
        if (this.OffhandSlot != null)
        {
            slots = slots.Append(this.OffhandSlot);
        }
        return slots.ToArray();
    }

    /// <summary>
    /// Returns only the slots of the inventory part of this window
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public Slot[] GetInventorySlots()
    {
        if (this.Inventory == null)
        {
            throw new NotSupportedException("Inventory is null");
        }
        return this.Inventory.GetContainerSlots()
            .Select(x => 
                new Slot(x.Item, (short)(x.SlotIndex + this.Slots.Length)))
            .ToArray();
    }

    /// <summary>
    /// Returns all slots in this window
    /// </summary>
    /// <returns></returns>
    public Slot[] GetAllSlots()
    {
        List<Slot> slots = new List<Slot>();
        slots.AddRange(this.Slots);

        if (this.Inventory != null)
        {
            slots.AddRange(
                this.Inventory.Slots
                    .Select(x => 
                        new Slot(x.Item, (short)(x.SlotIndex + this.Slots.Length))));
        }
        
        if (this.OffhandSlot != null)
            slots.Add(this.OffhandSlot);

        return slots.ToArray();
    }

    /// <summary>
    /// Returns the slot at the given index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Slot GetSlot(short index)
    {
        ThrowIfSlotOufOfRange(index);

        if (index == -1)
            return this.GetSelectedSlot().Clone();
        
        if (index == 45 && this.OffhandSlot != null)
            return this.OffhandSlot;

        if (index < this.Slots.Length)
            return this.Slots[index].Clone();

        var slot = this.Inventory!.GetSlot((short)(index - this.Slots.Length));
        slot.SlotIndex = index;
        return slot;

    }
    
    /// <summary>
    /// Sets the slot at the given index
    /// </summary>
    /// <param name="slot"></param>
    public void SetSlot(Slot slot)
    {
        ThrowIfSlotOufOfRange(slot.SlotIndex);

        if (slot.Item != null && slot.Item.Count == 0)
            slot.Item = null;

        if (slot.SlotIndex == -1)
        {
            this.SetSelectedSlot(slot);
        } else if (slot.SlotIndex == 45 && this.OffhandSlot != null)
        {
            this.OffhandSlot = slot;
        } else if (slot.SlotIndex < this.Slots.Length)
        {
            this.Slots[slot.SlotIndex] = slot;
        } else
        {
            slot.SlotIndex -= (short)this.Slots.Length;
            this.Inventory!.SetSlot(slot);
            return;
        }
        OnSlotChanged?.Invoke(this, slot.SlotIndex);
    }


    #endregion

    #region Helper API methods

    private IEnumerable<Slot> FindItem(int itemId, Slot[] slots)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].Item != null && slots[i].Item!.Info.Id == itemId)
            {
                yield return slots[i];
            }
        }
    }

    /// <summary>
    /// Returns inventory slots with the given item type in the inventory slots of this window
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    public IEnumerable<Slot> FindInventoryItems(int itemId) => this.FindItem(itemId, this.GetInventorySlots());

    /// <summary>
    /// Returns slots with the given item type in the container slots of this window
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    public IEnumerable<Slot> FindContainerItems(int itemId) => this.FindItem(itemId, this.GetContainerSlots());


    private IEnumerable<Slot> FindEmptySlots(Slot[] slots)
    {
        return slots.Where(x => x.Item == null);
    }

    /// <summary>
    /// Returns empty slots in the inventory section of the window
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Slot> FindEmptyInventorySlots() => this.FindEmptySlots(this.GetInventorySlots());

    /// <summary>
    /// Returns empty slots in the container section of the inventory
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Slot> FindEmptyContainerSlots() => this.FindEmptySlots(this.GetContainerSlots());

    public bool IsContainerSlotIndex(short slotIndex) => slotIndex < this.Slots.Length || (this.OffhandSlot != null && slotIndex == 45);

    private IEnumerable<Slot> FindSlotsToStack(int itemId, int count, Slot[] slots)
    {
        int left = count;
        foreach (var slot in FindItem(itemId, slots))
        {
            if (slot.IsFull())
            {
                continue;
            }

            yield return slot;
            left -= slot.LeftToStack;

            if (left <= 0)
            {
                yield break;
            }
        }

        yield return FindEmptySlots(slots).First();
    }

    public IEnumerable<Slot> FindInventorySlotsToStack(int itemId, int count) 
        => this.FindSlotsToStack(itemId, count, this.GetInventorySlots());
    
    public IEnumerable<Slot> FindContainerSlotsToStack(int itemId, int count) 
        => this.FindSlotsToStack(itemId, count, this.GetContainerSlots());

    
    private void StackSelectedSlot(int stackCount, IEnumerable<Slot> stackableSlots)
    {
        if (this.GetSelectedSlot().IsEmpty())
        {
            Logger.Warn("StackSelectedSlot() called, although GetSelectedSlot() is empty");
            return;
        }

        int expectedEndCount = this.GetSelectedSlot().Item!.Count - stackCount; 

        int left = stackCount;
        foreach (var slot in stackableSlots)
        {
            if (slot.IsEmpty() || slot.LeftToStack >= left)
            {
                // put down all items at once
                if (left == this.GetSelectedSlot().Item!.Count)
                {
                    this.DoSimpleClick(WindowMouseButton.MouseLeft, slot.SlotIndex);
                    return;
                }
                
                // put down <left> items one by one 
                for (int i = 0; i < left; i++)
                {
                    DoSimpleClick(WindowMouseButton.MouseRight, slot.SlotIndex);
                }

                if ((this.GetSelectedSlot().Item?.Count ?? 0) != expectedEndCount)
                {
                    throw new InvalidOperationException(
                        $"Expected selected slot to be {expectedEndCount}, but it was {this.GetSelectedSlot().Item?.Count.ToString() ?? "null"}");
                }
                return;
            } else
            {
                left -= slot.LeftToStack;
                this.DoSimpleClick(WindowMouseButton.MouseLeft, slot.SlotIndex);
            }
        }

        throw new Exception();
    }

    /// <summary>
    /// Stacks <see cref="GetSelectedSlot"/> in the inventory 
    /// </summary>
    /// <param name="stackCount"></param>
    public void StackSelectedSlotInInventory(int stackCount)
    {
        var selectedSlot = this.GetSelectedSlot();

        if (selectedSlot.IsEmpty())
        {
            return;
        }
        
        this.StackSelectedSlot(stackCount, 
            this.FindInventorySlotsToStack(selectedSlot.Item!.Info.Id, selectedSlot.Item!.Count));
    }
    
    /// <summary>
    /// Stacks <see cref="GetSelectedSlot"/> in the container 
    /// </summary>
    /// <param name="stackCount"></param>
    public void StackSelectedSlotInContainer(int stackCount)
    {
        var selectedSlot = this.GetSelectedSlot();

        if (selectedSlot.IsEmpty())
        {
            return;
        }
        
        this.StackSelectedSlot(stackCount, 
            this.FindContainerSlotsToStack(selectedSlot.Item!.Info.Id, selectedSlot.Item!.Count));
    }
    
    /// <summary>
    /// Picks up <paramref name="count"/> items from <paramref name="slot"/>. The items will be in the <see cref="GetSelectedSlot"/>
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="count"></param>
    public void PickupItems(short slot, int count)
    {
        var pickupSlot = this.GetSlot(slot);
        var selectedSlot = this.GetSelectedSlot();

        if (pickupSlot.IsEmpty())
        {
            return;
        }

        if (pickupSlot.Item!.Count < count)
        {
            throw new InvalidOperationException($"cannot pickup {count} items from slot {pickupSlot}");
        }
        
        if (!selectedSlot.IsEmpty())
        {
            throw new Exception("Cannot pickup items from slot, because GetSelectedSlot() cannot stack this item");
        }

        if (pickupSlot.Item!.Count >= 2 * count)
        {
            // pickup half stack
            this.DoSimpleClick(WindowMouseButton.MouseRight, slot);
        } else
        {
            // pickup full stack
            this.DoSimpleClick(WindowMouseButton.MouseLeft, slot);
        }
        
        selectedSlot = this.GetSelectedSlot();

        // if more picked up than needed, put the rest down
        if (selectedSlot.Item!.Count > count)
        {
            int toPutDown = selectedSlot.Item!.Count - count;
            
            if (this.IsContainerSlotIndex(slot))
            {
                this.StackSelectedSlotInContainer(toPutDown);
            } else
            {
                this.StackSelectedSlotInInventory(toPutDown);
            }
        }

        selectedSlot = this.GetSelectedSlot();
        if (selectedSlot.Item!.Count != count)
        {
            throw new InvalidOperationException(
                $"Result count does not match expected output (got: {selectedSlot.Item!.Count}, expected: {count}");
        }
    }
    
    /// <summary>
    /// Moves <paramref name="count"/> items from slot <paramref name="slotFromIndex"/> to <paramref name="slotToIndex"/>
    /// </summary>
    /// <param name="slotFromIndex"></param>
    /// <param name="slotToIndex"></param>
    /// <param name="count"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public void MoveItemsFromSlot(short slotFromIndex, short slotToIndex, int count)
    {
        var slotFrom = this.GetSlot(slotFromIndex);
        var slotTo = this.GetSlot(slotToIndex);

        if (slotFrom.Item == null || slotFrom.Item.Count < count)
        {
            throw new ArgumentException($"Slot {slotFrom} does not have enough items");
        }

        if (!slotTo.CanStack(slotFrom, count))
        {
            throw new ArgumentException($"Slot {slotTo} cannot stack {count} items of slot {slotFrom}");
        }

        if (!this.GetSelectedSlot().IsEmpty())
        {
            throw new InvalidOperationException("Selected slot must be empty");
        }
        
        this.PickupItems(slotFromIndex, count);
        this.DoSimpleClick(WindowMouseButton.MouseLeft, slotToIndex);
    }


    #endregion
    
    #region Clicks

    /// <summary>
    /// Performs a simple click on this window
    /// </summary>
    /// <param name="button"></param>
    /// <param name="clickedSlot"></param>
    /// <exception cref="ArgumentException"></exception>
    public void DoSimpleClick(WindowMouseButton button, short clickedSlot)
    {
        //ThrowIfSlotOufOfRange(clickedSlot);
        if (button != WindowMouseButton.MouseLeft && button != WindowMouseButton.MouseRight)
        {
            throw new ArgumentException($"{nameof(SimpleWindowClick)} does not support mouse button {Enum.GetName(button)}");
        }

        var click = new SimpleWindowClick(this, clickedSlot, (byte)button);
        this.PerformWindowClick(click);
    }
    
    
    private void PerformWindowClick(WindowClick click)
    {
        if (this.IsSynchronized)
        {
            lock (this._syncLock)
            {
                click.PerformClick();
                this.Synchronizer!(this, click).Wait();
            }
        } else
        {
            click.PerformClick();
        }
    }

    #endregion

    private void ThrowIfSlotOufOfRange(short index, bool allowSelectedSlot = true)
    {
        int count = this.TotalSlotCount;
        if ((!allowSelectedSlot && index == -1) || (index != -1 && (index < 0 || index >= count)))
        {
            throw new IndexOutOfRangeException(nameof(index) + $" is out of range ({index} / {count})");
        }
    }

    
    public override string ToString()
    {
        return $"Window (Id={this.WindowId}, State={this.StateId}, Title={this.Title}, Slots={this.Slots.Length}, HasInventory={this.Inventory != null})";
    }
}
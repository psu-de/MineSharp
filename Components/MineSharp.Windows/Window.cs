using MineSharp.Core.Common;
using MineSharp.Core.Common.Items;
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
    private object SyncLock { get; }
    private bool IsSynchronized => this.Synchronizer != null;
    
    public event SlotChanged? OnSlotChanged;
    
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
        this.SyncLock = new object();

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

        if (slot.Item?.Count == 0)
            slot.Item = null;

        var index = slot.SlotIndex;

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
        }
        
        this.OnSlotChanged?.Invoke(this, index);
    }


    #endregion

    #region Helper API methods

    private IEnumerable<Slot> FindItem(ItemType item, Slot[] slots)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].Item != null && slots[i].Item!.Info.Type == item)
            {
                yield return slots[i];
            }
        }
    }

    /// <summary>
    /// Returns inventory slots with the given item type in the inventory slots of this window
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public IEnumerable<Slot> FindInventoryItems(ItemType item) => this.FindItem(item, this.GetInventorySlots());

    /// <summary>
    /// Returns slots with the given item type in the container slots of this window
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public IEnumerable<Slot> FindContainerItems(ItemType item) => this.FindItem(item, this.GetContainerSlots());


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

    /// <summary>
    /// Whether the given slot index is in the container range of this window
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <returns></returns>
    public bool IsContainerSlotIndex(short slotIndex) => slotIndex < this.Slots.Length || (this.OffhandSlot != null && slotIndex == 45);

    private IEnumerable<Slot> FindSlotsToStack(ItemType item, int count, Slot[] slots)
    {
        int left = count;
        foreach (var slot in FindItem(item, slots))
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

    /// <summary>
    /// Yields slots in the Inventory where the given item can be stacked upon.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public IEnumerable<Slot> FindInventorySlotsToStack(ItemType item, int count) 
        => this.FindSlotsToStack(item, count, this.GetInventorySlots());
    
    /// <summary>
    /// Yields slots in the Container where the given item can be stacked upon
    /// </summary>
    /// <param name="item"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public IEnumerable<Slot> FindContainerSlotsToStack(ItemType item, int count) 
        => this.FindSlotsToStack(item, count, this.GetContainerSlots());

    
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
            this.FindInventorySlotsToStack(selectedSlot.Item!.Info.Type, selectedSlot.Item!.Count));
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
            this.FindContainerSlotsToStack(selectedSlot.Item!.Info.Type, selectedSlot.Item!.Count));
    }

    /// <summary>
    /// Put down the <see cref="count"/> items in <see cref="GetSelectedSlot"/> to the slot given
    /// </summary>
    /// <param name="count"></param>
    /// <param name="slot"></param>
    public void PutDownItems(int count, short slot)
    {
        var selectedSlot = this.GetSelectedSlot();
        var stackSlot = this.GetSlot(slot);
        
        if (selectedSlot.IsEmpty())
        {
            throw new InvalidOperationException("selected slot is empty");
        }
        
        if (!stackSlot.CanStack(selectedSlot, count))
        {
            throw new InvalidOperationException($"cannot stack {count} {selectedSlot.Item!.Info.Type} on the given slot");
        }

        if (selectedSlot.Item!.Count == count)
        {
            // Put down the whole stack at once
            this.DoSimpleClick(WindowMouseButton.MouseLeft, slot);
            return;
        }
        
        // put down items one by one
        // TODO: PutDownItems(): this could be done with fewer clicks
        for (int i = 0; i < count; i++)
        {
            this.DoSimpleClick(WindowMouseButton.MouseRight, slot);
        }
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

        // if more picked up than needed, put the rest down on the same slot
        if (selectedSlot.Item!.Count > count)
        {
            int toPutDown = selectedSlot.Item!.Count - count;
            this.PutDownItems(toPutDown, pickupSlot.SlotIndex);
        }
        
        if (selectedSlot.Item!.Count != count)
        {
            throw new InvalidOperationException(
                $"Result count does not match expected output (got: {selectedSlot.Item!.Count}, expected: {count}");
        }
    }
    
    /// <summary>
    /// Picks up <paramref name="count"/> items of type <paramref name="type"/> from the container range. The items will be in the <see cref="GetSelectedSlot"/>
    /// </summary>
    /// <param name="type"></param>
    /// <param name="count"></param>
    public void PickupContainerItems(ItemType type, int count)
        => PickupItems(type, count, this.GetContainerSlots());

    /// <summary>
    /// Picks up <paramref name="count"/> items of type <paramref name="type"/> from the inventory range. The items will be in the <see cref="GetSelectedSlot"/>
    /// </summary>
    /// <param name="type"></param>
    /// <param name="count"></param>
    public void PickupInventoryItems(ItemType type, int count)
        => PickupItems(type, count, this.GetInventorySlots());
    
    private void PickupItems(ItemType type, int count, IEnumerable<Slot> enumerable)
    {
        if (!this.GetSelectedSlot().IsEmpty())
        {
            throw new InvalidOperationException("Selected slot must be empty");
        }
        
        var slots = enumerable as Slot[] ?? enumerable.ToArray();
        
        if (count > CountItems(type, slots))
        {
            throw new InvalidOperationException($"Not enough items {type} in this window.");
        }
        
        var possibleSlots = slots.Where(x => x.Item?.Info.Type == type).ToArray();
        if (possibleSlots.Length == 0)
        {
            throw new InvalidOperationException();
        }
        
        var slot = possibleSlots.FirstOrDefault(x => x.Item?.Count >= count);
        if (null != slot)
        {
            PickupItems(slot.SlotIndex, count);
            return;
        }
        
        // move items to the first slot until the desired count is reached
        slot = possibleSlots.First();
        var current = (int)slot.Item!.Count;
        foreach (var pickupSlot in possibleSlots.Skip(1))
        {
            var toPickup = Math.Min(pickupSlot.Item!.Count, count - current);
            this.PickupItems(pickupSlot.SlotIndex, toPickup);
            this.PutDownItems(toPickup, slot.SlotIndex);
            
            current += toPickup;
            if (current == count)
                break;
        }

        if (current != count)
        {
            throw new InvalidOperationException($"something went wrong. Picked up {current} items instead of {count}");
        }
        
        // now, exactly <count> items are on <slot>, and they can be picked up
        this.PickupItems(slot.SlotIndex, count);
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

    /// <summary>
    /// Counts how many items of the given type are in this window (Container + Inventory)
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int CountItems(ItemType type)
    {
        return CountItems(type, this.GetAllSlots());
    }

    private int CountItems(ItemType type, IEnumerable<Slot> slots)
    {
        return slots
            .Where(x => x.Item?.Info.Type == type)
            .Select(x => (int)x.Item!.Count)
            .Sum();
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
            lock (this.SyncLock)
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
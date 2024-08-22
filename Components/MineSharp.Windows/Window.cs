using MineSharp.Core.Common;
using MineSharp.Core.Common.Items;
using MineSharp.Core.Events;
using MineSharp.Data.Windows;
using MineSharp.Windows.Clicks;
using NLog;

namespace MineSharp.Windows;

/// <summary>
///     Represents a Minecraft window
/// </summary>
public class Window
{
    /// <summary>
    ///     Fired when a slot has changed
    /// </summary>
    public delegate void SlotChanged(Window window, short index);

    /// <summary>
    ///     Delegate to synchronize a window when it has been clicked
    /// </summary>
    public delegate Task WindowSynchronizer(Window window, WindowClick click);

    internal const short SelectedSlot = -1;
    internal const short OffhandSlot = 45;
    internal const byte InventoryId = 0;
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    ///     Create a new window instance
    /// </summary>
    /// <param name="id"></param>
    /// <param name="title"></param>
    /// <param name="uniqueSlotCount"></param>
    /// <param name="inventory"></param>
    /// <param name="windowSynchronizer"></param>
    public Window(byte id, string title, int uniqueSlotCount, Window? inventory = null,
                  WindowSynchronizer? windowSynchronizer = null)
    {
        WindowId = id;
        Title = title;
        Inventory = inventory;
        SlotCount = (short)uniqueSlotCount;
        HasOffhandSlot = id == InventoryId; // Only main inventory has offhand slot
        Synchronizer = windowSynchronizer;
        SyncLock = new();

        Slots = InitializeSlots(uniqueSlotCount);

        if (inventory == null)
        {
            Slots.Add(SelectedSlot, null);
        }

        if (HasOffhandSlot)
        {
            Slots.Add(OffhandSlot, null);
        }
    }

    /// <summary>
    ///     Numerical id to identify the window
    /// </summary>
    public byte WindowId { get; }

    /// <summary>
    ///     Window state id used to synchronize the window
    /// </summary>
    public int StateId { get; set; }

    /// <summary>
    ///     The title of the window
    /// </summary>
    public string Title { get; }

    /// <summary>
    ///     Number of slots in this window owned by this window
    /// </summary>
    public short SlotCount { get; }

    /// <summary>
    ///     Whether this window has an offhand slot
    /// </summary>
    public bool HasOffhandSlot { get; }

    /// <summary>
    ///     Window extensions (usually the player's inventory)
    /// </summary>
    public Window? Inventory { get; }

    /// <summary>
    ///     Total number of slots (including window extension)
    /// </summary>
    public int TotalSlotCount
    {
        get
        {
            int count = SlotCount;
            count += Inventory?.SlotCount ?? 0;
            count += HasOffhandSlot ? 1 : 0;
            return count;
        }
    }

    private IDictionary<short, Item?> Slots { get; }
    private WindowSynchronizer? Synchronizer { get; }
    private object SyncLock { get; }
    private bool IsSynchronized => Synchronizer != null;

    /// <summary>
    ///     Fired when a slot changed
    /// </summary>
    public AsyncEvent<Window, short> OnSlotChanged = new();

    private IDictionary<short, Item?> InitializeSlots(int count)
    {
        var dict = new Dictionary<short, Item?>();
        for (short i = 0; i < count; i++)
        {
            dict.Add(i, null);
        }

        return dict;
    }

    private void ThrowIfSlotOufOfRange(short index, bool allowSelectedSlot = true)
    {
        var count = TotalSlotCount;
        if ((!allowSelectedSlot && index == -1) || (index != -1 && (index < 0 || index >= count)))
        {
            throw new IndexOutOfRangeException(nameof(index) + $" is out of range ({index} / {count})");
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return
            $"Window (Id={WindowId}, State={StateId}, Title={Title}, Slots={SlotCount}, HasInventory={Inventory != null})";
    }

    #region Get / Set slots

    /// <summary>
    ///     Sets the selected slot
    /// </summary>
    /// <param name="item"></param>
    private void SetSelectedSlot(Item? item)
    {
        if (Inventory != null)
        {
            Inventory.SetSelectedSlot(item);
        }
        else
        {
            Slots[SelectedSlot] = item;
        }
    }

    /// <summary>
    ///     Returns the selected slot
    /// </summary>
    /// <returns></returns>
    public Slot GetSelectedSlot()
    {
        return Inventory != null
            ? Inventory.GetSelectedSlot()
            : new(Slots[SelectedSlot], SelectedSlot);
    }

    /// <summary>
    ///     Returns only the container slots of this window
    /// </summary>
    /// <returns></returns>
    public Slot[] GetContainerSlots()
    {
        return Slots
              .Where(kvp => kvp.Key is not SelectedSlot)
              .Select(x => new Slot(x.Value, x.Key))
              .ToArray();
    }

    /// <summary>
    ///     Returns only the slots of the inventory part of this window
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public Slot[] GetInventorySlots()
    {
        if (Inventory == null)
        {
            throw new InvalidOperationException("inventory is null");
        }

        return Inventory
              .GetContainerSlots()
              .Select(x =>
               {
                   x.SlotIndex += SlotCount;
                   return x;
               })
              .ToArray();
    }

    /// <summary>
    ///     Returns only the hotbar slots of the inventory part of this window.
    ///     Only works for player inventories
    /// </summary>
    /// <returns></returns>
    public Slot[] GetHotbarSlots()
    {
        return GetInventorySlots()
            .Where(x => x.SlotIndex >= (short)PlayerWindowSlots.HotbarStart && x.SlotIndex <= (short)PlayerWindowSlots.HotbarEnd)
            .ToArray();
    }

    /// <summary>
    ///     Returns all slots in this window
    /// </summary>
    /// <returns></returns>
    public Slot[] GetAllSlots()
    {
        var slots = new List<Slot>();
        if (Inventory != null)
        {
            slots.AddRange(GetInventorySlots());
        }

        slots.AddRange(GetContainerSlots());

        return slots.ToArray();
    }

    /// <summary>
    ///     Returns the slot at the given index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Slot GetSlot(short index)
    {
        ThrowIfSlotOufOfRange(index);

        if (index == SelectedSlot)
        {
            return GetSelectedSlot();
        }

        if (index < SlotCount || (index == OffhandSlot && HasOffhandSlot))
        {
            return new(Slots[index], index);
        }

        var slot = Inventory!.GetSlot((short)(index - SlotCount));
        slot.SlotIndex = index;
        return slot;
    }

    /// <summary>
    ///     Sets the slot at the given index
    /// </summary>
    /// <param name="slot"></param>
    public void SetSlot(Slot slot)
    {
        UpdateSlot(slot.Item, slot.SlotIndex);
    }

    internal void UpdateSlot(Item? item, short slotIndex)
    {
        lock (SyncLock)
        {
            ThrowIfSlotOufOfRange(slotIndex);

            if (item?.Count <= 0)
            {
                item = null;
            }

            if (slotIndex == SelectedSlot)
            {
                SetSelectedSlot(item);
            }
            else if (slotIndex < SlotCount || (slotIndex == OffhandSlot && HasOffhandSlot))
            {
                Slots[slotIndex] = item;
            }
            else
            {
                Inventory!.UpdateSlot(item, (short)(slotIndex - SlotCount));
            }
        }

        OnSlotChanged.Dispatch(this, slotIndex);
    }

    /// <summary>
    ///     Update all slots in the window
    /// </summary>
    /// <param name="slots"></param>
    public void SetSlots(Slot[] slots)
    {
        if (slots.Length != TotalSlotCount)
        {
            throw new InvalidOperationException(
                $"{nameof(slots)}.Length must equal {nameof(TotalSlotCount)} (expected {TotalSlotCount}, got {slots.Length})");
        }

        for (short i = 0; i < slots.Length; i++)
        {
            SetSlot(slots[i]);
        }
    }

    #endregion

    #region Helper API methods

    private IEnumerable<Slot> FindItem(ItemType item, IEnumerable<Slot> slots)
    {
        return slots.Where(x => x.Item?.Info.Type == item);
    }

    /// <summary>
    ///     Returns inventory slots with the given item type in the inventory slots of this window
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public IEnumerable<Slot> FindInventoryItems(ItemType item)
    {
        return FindItem(item, GetInventorySlots());
    }

    /// <summary>
    ///     Returns slots with the given item type in the container slots of this window
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public IEnumerable<Slot> FindContainerItems(ItemType item)
    {
        return FindItem(item, GetContainerSlots());
    }


    private IEnumerable<Slot> FindEmptySlots(Slot[] slots)
    {
        return slots.Where(x => x.Item == null);
    }

    /// <summary>
    ///     Returns empty slots in the inventory section of the window
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Slot> FindEmptyInventorySlots()
    {
        return FindEmptySlots(GetInventorySlots());
    }

    /// <summary>
    ///     Returns empty slots in the container section of the inventory
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Slot> FindEmptyContainerSlots()
    {
        return FindEmptySlots(GetContainerSlots());
    }

    /// <summary>
    ///     Whether the given slot index is in the container range of this window
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <returns></returns>
    public bool IsContainerSlotIndex(short slotIndex)
    {
        return slotIndex < SlotCount || (HasOffhandSlot && slotIndex == OffhandSlot);
    }

    private IEnumerable<Slot> FindSlotsToStack(ItemType item, int count, Slot[] slots)
    {
        var left = count;
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
    ///     Yields slots in the Inventory where the given item can be stacked upon.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public IEnumerable<Slot> FindInventorySlotsToStack(ItemType item, int count)
    {
        return FindSlotsToStack(item, count, GetInventorySlots());
    }

    /// <summary>
    ///     Yields slots in the Container where the given item can be stacked upon
    /// </summary>
    /// <param name="item"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public IEnumerable<Slot> FindContainerSlotsToStack(ItemType item, int count)
    {
        return FindSlotsToStack(item, count, GetContainerSlots());
    }


    private void StackSelectedSlot(int stackCount, IEnumerable<Slot> stackableSlots)
    {
        if (GetSelectedSlot().IsEmpty())
        {
            Logger.Warn("StackSelectedSlot() called, although GetSelectedSlot() is empty");
            return;
        }

        var expectedEndCount = GetSelectedSlot().Item!.Count - stackCount;

        var left = stackCount;
        foreach (var slot in stackableSlots)
        {
            if (slot.IsEmpty() || slot.LeftToStack >= left)
            {
                // put down all items at once
                if (left == GetSelectedSlot().Item!.Count)
                {
                    DoSimpleClick(WindowMouseButton.MouseLeft, slot.SlotIndex);
                    return;
                }

                // put down <left> items one by one 
                for (var i = 0; i < left; i++)
                {
                    DoSimpleClick(WindowMouseButton.MouseRight, slot.SlotIndex);
                }

                if ((GetSelectedSlot().Item?.Count ?? 0) != expectedEndCount)
                {
                    throw new InvalidOperationException(
                        $"expected selected slot to be {expectedEndCount}, but it was {GetSelectedSlot().Item?.Count.ToString() ?? "null"}");
                }

                return;
            }

            left -= slot.LeftToStack;
            DoSimpleClick(WindowMouseButton.MouseLeft, slot.SlotIndex);
        }

        throw new();
    }

    /// <summary>
    ///     Stacks <see cref="GetSelectedSlot" /> in the inventory
    /// </summary>
    /// <param name="stackCount"></param>
    public void StackSelectedSlotInInventory(int stackCount)
    {
        var selectedSlot = GetSelectedSlot();

        if (selectedSlot.IsEmpty())
        {
            return;
        }

        StackSelectedSlot(stackCount,
                          FindInventorySlotsToStack(selectedSlot.Item!.Info.Type, selectedSlot.Item!.Count));
    }

    /// <summary>
    ///     Stacks <see cref="GetSelectedSlot" /> in the container
    /// </summary>
    /// <param name="stackCount"></param>
    public void StackSelectedSlotInContainer(int stackCount)
    {
        var selectedSlot = GetSelectedSlot();

        if (selectedSlot.IsEmpty())
        {
            return;
        }

        StackSelectedSlot(stackCount,
                          FindContainerSlotsToStack(selectedSlot.Item!.Info.Type, selectedSlot.Item!.Count));
    }

    /// <summary>
    ///     Put down the <paramref name="count" /> items in <see cref="GetSelectedSlot" /> to the slot given
    /// </summary>
    /// <param name="count"></param>
    /// <param name="slot"></param>
    public void PutDownItems(int count, short slot)
    {
        var selectedSlot = GetSelectedSlot();
        var stackSlot = GetSlot(slot);

        if (selectedSlot.IsEmpty())
        {
            throw new InvalidOperationException("selected slot is empty");
        }

        if (!stackSlot.CanStack(selectedSlot, count))
        {
            throw new InvalidOperationException(
                $"cannot stack {count} {selectedSlot.Item!.Info.Type} on the given slot");
        }

        if (selectedSlot.Item!.Count == count)
        {
            // Put down the whole stack at once
            DoSimpleClick(WindowMouseButton.MouseLeft, slot);
            return;
        }

        // put down items one by one
        // TODO: PutDownItems(): this could be done with fewer clicks
        for (var i = 0; i < count; i++)
        {
            DoSimpleClick(WindowMouseButton.MouseRight, slot);
        }
    }

    /// <summary>
    ///     Picks up <paramref name="count" /> items from <paramref name="slot" />. The items will be in the
    ///     <see cref="GetSelectedSlot" />
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="count"></param>
    public void PickupItems(short slot, int count)
    {
        var pickupSlot = GetSlot(slot);
        var selectedSlot = GetSelectedSlot();

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
            throw new("Cannot pickup items from slot, because GetSelectedSlot() must be empty");
        }

        if (pickupSlot.Item!.Count >= 2 * count)
        {
            // pickup half stack
            DoSimpleClick(WindowMouseButton.MouseRight, slot);
        }
        else
        {
            // pickup full stack
            DoSimpleClick(WindowMouseButton.MouseLeft, slot);
        }

        selectedSlot = GetSelectedSlot();

        // if more picked up than needed, put the rest down on the same slot
        if (selectedSlot.Item!.Count > count)
        {
            var toPutDown = selectedSlot.Item!.Count - count;
            PutDownItems(toPutDown, pickupSlot.SlotIndex);
        }

        if (selectedSlot.Item!.Count != count)
        {
            throw new InvalidOperationException(
                $"Result count does not match expected output (got: {selectedSlot.Item!.Count}, expected: {count}");
        }
    }

    /// <summary>
    ///     Picks up <paramref name="count" /> items of type <paramref name="type" /> from the container range. The items will
    ///     be in the <see cref="GetSelectedSlot" />
    /// </summary>
    /// <param name="type"></param>
    /// <param name="count"></param>
    public void PickupContainerItems(ItemType type, int count)
    {
        PickupItems(type, count, GetContainerSlots());
    }

    /// <summary>
    ///     Picks up <paramref name="count" /> items of type <paramref name="type" /> from the inventory range. The items will
    ///     be in the <see cref="GetSelectedSlot" />
    /// </summary>
    /// <param name="type"></param>
    /// <param name="count"></param>
    public void PickupInventoryItems(ItemType type, int count)
    {
        PickupItems(type, count, GetInventorySlots());
    }

    private void PickupItems(ItemType type, int count, IEnumerable<Slot> enumerable)
    {
        if (!GetSelectedSlot().IsEmpty())
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
            PickupItems(pickupSlot.SlotIndex, toPickup);
            PutDownItems(toPickup, slot.SlotIndex);

            current += toPickup;
            if (current == count)
            {
                break;
            }
        }

        if (current != count)
        {
            throw new InvalidOperationException($"something went wrong. Picked up {current} items instead of {count}");
        }

        // now, exactly <count> items are on <slot>, and they can be picked up
        PickupItems(slot.SlotIndex, count);
    }

    /// <summary>
    ///     Counts how many items of the given type are in this window (Container + Inventory)
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int CountItems(ItemType type)
    {
        return CountItems(type, GetAllSlots());
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
    ///     Performs a simple click on this window
    /// </summary>
    /// <param name="button"></param>
    /// <param name="clickedSlot"></param>
    /// <exception cref="ArgumentException"></exception>
    public void DoSimpleClick(WindowMouseButton button, short clickedSlot)
    {
        //ThrowIfSlotOufOfRange(clickedSlot);
        if (button != WindowMouseButton.MouseLeft && button != WindowMouseButton.MouseRight)
        {
            throw new ArgumentException(
                $"{nameof(SimpleWindowClick)} does not support mouse button {Enum.GetName(button)}");
        }

        var click = new SimpleWindowClick(this, clickedSlot, (byte)button);
        PerformWindowClick(click);
    }


    private void PerformWindowClick(WindowClick click)
    {
        if (IsSynchronized)
        {
            lock (SyncLock)
            {
                click.PerformClick();
                Synchronizer!(this, click).Wait();
            }
        }
        else
        {
            click.PerformClick();
        }
    }

    #endregion
}

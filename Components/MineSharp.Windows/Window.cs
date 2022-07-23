using System.Collections.Specialized;
using System.Security.Cryptography.X509Certificates;
using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Items;
using MineSharp.Data.Windows;

namespace MineSharp.Windows {
    public class Window : IDisposable {

        public const int InventorySlotCount = 27;
        public const int HotbarSlotCount = 9;

        private static readonly Logger Logger = Logger.GetLogger();

        public delegate void WindowClosedEvent(Window window);
        public delegate void WindowClickedEvent(Window sender, WindowClick click);
        public delegate void WindowSlotEvent(Window sender, int slotIndex);

        /// <summary>
        /// This event fires when the window has already been disposed.
        /// </summary>
        public event WindowClosedEvent? WindowClosed;
        public event WindowClickedEvent? WindowClicked;
        public event WindowSlotEvent? WindowSlotUpdated;

        public WindowInfo Info { get; private set; }

        public int ContainerStart => 0;
        public int ContainerEnd => Info.UniqueSlots - 1;
        public int InventoryStart => Info.ExcludeInventory ? throw new NotSupportedException() : Info.UniqueSlots;
        public int InventoryEnd => InventoryStart + InventorySlotCount - 1;
        public int HotbarStart => InventoryEnd + 1;
        public int HotbarEnd => HotbarStart + HotbarSlotCount - 1;
        public int TotalSlotCount => (Info.ExcludeInventory ? Info.UniqueSlots : HotbarEnd + 1) + (Info.HasOffHandSlot ? 1 : 0);


        public int Id { get; private set; }
        private Slot[] ContainerSlots { get; set; }
        private Slot? OffHandSlot { get; set; }

        public Window? InventoryWindow { get; private set; }

        public Slot? SelectedSlot { get; set; }
        public int StateId { get; set; }


        public Window(WindowInfo info, Slot[]? slots = null, Window? playerInventory = null) {
            this.Info = info;
            this.InventoryWindow = playerInventory;

            if (slots != null && slots.Length != info.UniqueSlots)
                throw new ArgumentException($"Invalid slot count ({slots.Length}) for window ({Info.Name}). Expected {TotalSlotCount}");

            if (slots == null) {
                this.ContainerSlots = new Slot[info.UniqueSlots];
                for (int i = 0; i < info.UniqueSlots; i++) {
                    this.ContainerSlots[i] = new Slot(null, (short)i);
                }
            } else this.ContainerSlots = slots;

            if (info.HasOffHandSlot)
                OffHandSlot = new Slot(null, (short)(TotalSlotCount - 1));
        }

        public Window(int windowId, WindowInfo info, Slot[]? slots = null, int? stateId = null, Window? playerInventory = null) : this(info, slots, playerInventory) {
            this.Id = windowId;

            this.StateId = stateId ?? 0;
            this.SelectedSlot = new Slot(null, -1);
        }

        internal void SwapSelectedSlot(int slotNumber) {
            Slot t = this.SelectedSlot!;
            this.SelectedSlot = GetSlot(slotNumber).Clone();
            this.SelectedSlot.SlotNumber = -1;

            t.SlotNumber = (short)slotNumber;
            this.SetSlot(t);
        }

        public void PerformClick(WindowClick click) {
            click.PerformClick(this);
            WindowClicked?.Invoke(this, click);
        }

        public void SetSlot(Slot slot) {

            if (OffHandSlot != null && OffHandSlot.SlotNumber == slot.SlotNumber) {
                OffHandSlot = slot;
                WindowSlotUpdated?.Invoke(this, (int)OffHandSlot.SlotNumber!);
                return;
            }

            if (slot.SlotNumber! >= ContainerSlots.Length) {
                if (this.Info.ExcludeInventory || this.InventoryWindow == null) throw new ArgumentOutOfRangeException("SlotNumber out of range");

                slot.SlotNumber -= (short)this.ContainerSlots.Length;
                this.InventoryWindow.SetSlot(slot);
                return;
            }

            this.ContainerSlots[(int)(slot.SlotNumber!)] = slot;
            WindowSlotUpdated?.Invoke(this, (int)slot.SlotNumber!);
        }

        public Slot GetSlot(int index) {
            if (OffHandSlot != null && OffHandSlot.SlotNumber == index)
                return OffHandSlot;

            if (index >= ContainerSlots.Length) {
                if (this.Info.ExcludeInventory || this.InventoryWindow == null) throw new ArgumentOutOfRangeException("index out of range");

                index -= (short)this.ContainerSlots.Length;
                return this.InventoryWindow.GetSlot(index);
            }

            return this.ContainerSlots[index];
        }

        public void UpdateSlots(Slot[] slotData) {
            foreach (var slot in slotData)
                this.SetSlot(slot);
        }

        public Slot[] GetContainerSlots() {
            List<Slot> slots = new List<Slot>();
            slots.AddRange(this.ContainerSlots);

            if (OffHandSlot != null)
                slots.Add(OffHandSlot);
            return slots.ToArray();
        }

        public Slot[] GetAllSlots() {
            List<Slot> slots = GetContainerSlots().ToList();

            if (this.Info.ExcludeInventory || this.InventoryWindow != null) {
                slots.AddRange(this.InventoryWindow!.GetContainerSlots().Select(x => new Slot(x.Item, (short)(x.SlotNumber! + this.ContainerSlots.Length))));
            }

            return slots.ToArray();
        }

        public Item?[] ContainerItems() => GetContainerSlots().Select(x => x.Item).ToArray();

        public Item?[] AllItems() => GetAllSlots().Select(x => x.Item).ToArray();

        /// <summary>
        /// When using MineSharp.Bot, please use Bot.CloseWindow(windowId) instead.
        /// </summary>
        public void Close() {
            this.Dispose();
            WindowClosed?.Invoke(this);
        }

        public void Dispose() {
            this.ContainerSlots = Array.Empty<Slot>();
            this.InventoryWindow = null;
        }

        public Slot[] EmptyContainerSlots() {
            List<Slot> emptySlots = new List<Slot>();
            emptySlots.AddRange(this.ContainerSlots.Where(x => x.IsEmpty()));
            if (OffHandSlot != null && OffHandSlot.IsEmpty())
                emptySlots.Add(OffHandSlot);
            return emptySlots.ToArray();
        }

        public Slot[] AllEmptySlots() {
            var emptySlots = EmptyContainerSlots().ToList();
            if (!this.Info.ExcludeInventory && InventoryWindow != null) {
                emptySlots.AddRange(InventoryWindow.EmptyContainerSlots());
            }
            return emptySlots.ToArray();
        }

        public int EmptySlotCount => EmptyContainerSlots().Length;
        public int AllEmptySlotCount => AllEmptySlots().Length;


        private Slot? FindItem(Slot[] slots, Item searched) => slots.FirstOrDefault(x => !x.IsEmpty() && x.Item!.Id == searched.Id);

        /// <summary>
        /// Searches through the container slots for an item
        /// </summary>
        /// <param name="itemInfo"></param>
        /// <returns></returns>
        public Slot? FindContainerItem(Item searched) => FindItem(this.ContainerSlots, searched);

        public Slot? FindInventoryItem(Item searched) => FindItem(this.InventoryWindow!.ContainerSlots, searched);

        private void SwitchSlots(short slot1, short slot2) {
            if (slot1 == slot2) return;

            Slot a = GetSlot(slot1);
            Slot b = GetSlot(slot2);

            if (a.IsEmpty() && b.IsEmpty())
                return;

            var click = new WindowClick(WindowOperationMode.SimpleClick, (byte)WindowMouseButton.MouseLeft, a.IsEmpty() ? slot2 : slot1);
            this.PerformClick(click);

            click = new WindowClick(WindowOperationMode.SimpleClick, (byte)WindowMouseButton.MouseLeft, a.IsEmpty() ? slot1 : slot2);
            this.PerformClick(click);
        }

        /// <summary>
        /// Finds a slot in <paramref name="range"/> to stack <paramref name="item"/> on.
        /// Returns 0 when no slot has been found.
        /// </summary>
        /// <param name="range">Slot range</param>
        /// <param name="item">Item to stack</param>
        /// <returns>Yields slots until all items have been stacked</returns>
        private static IEnumerable<Slot> FindStackableSlots(Slot[] range, Item item) {
            // try to find an slot with the same item, or return an empty slot
            int left = item.Count;
            foreach (var slot in range.Where(x => !x.IsEmpty() && 
                                                    x.Item!.Id == item.Id && 
                                                    x.Item!.Metadata == item.Metadata &&
                                                    (x.Item.StackSize - x.Item.Count - item.Count) > 0)) {
                yield return slot;
                left -= Math.Min(left, (slot.Item!.StackSize - slot.Item!.Count));
                
                if (left <= 0) 
                    yield break;
            }

            var first = range.FirstOrDefault(x => x.IsEmpty());
            if (first == null)
                yield break;
            yield return first!;
        }

        public IEnumerable<Slot> FindStackableContainerSlots(Item item) => FindStackableSlots(ContainerSlots, item);

        public IEnumerable<Slot> FindStackableInventorySlots(Item item) => FindStackableSlots(GetAllSlots().Skip(InventoryStart).ToArray(), item);

        /// <summary>
        /// <para>Tries to take <paramref name="count"/> items of type <paramref name="itemId"/> from the container into the inventory</para>
        /// <para>Clicks yielded must be applied instantly. </para>
        /// <para>Getting the next click without applying the one before will result in unwanted behaviour.</para>
        /// </summary>
        /// <param name="itemId">The id of the item</param>
        /// <param name="count">Number of items to take. Null to take as many items as possible.</param>
        /// <param name="metadata">Optional metadata of the item. If null it will be ignored.</param>
        /// <param name="throwIfNotEnough">Throws if less than <paramref name="count"/> items were found</param>
        /// <returns></returns>
        public IEnumerable<WindowClick> TakeItem(int itemId, byte? count = null, fNbt.NbtCompound? metadata = null, bool throwIfNotEnough = false)
        {
            if (this.InventoryWindow == null)
                throw new InvalidOperationException("Window must be extended with inventory window");

            var searchedItems = this.GetContainerSlots().Where(x => x.Item != null && x.Item!.Id == itemId);
            if (metadata != null)
                searchedItems = searchedItems.Where(x => x.Item!.Metadata == metadata);
            if (count == null) throwIfNotEnough = false;

            byte left = count ?? byte.MaxValue;
            foreach (var slot in searchedItems) {
                byte max = Math.Min(left, slot.Item!.Count);

                var needle = ItemFactory.CreateItem(itemId, max, slot.Item!.Damage, slot.Item!.Metadata);

                foreach (var stackSlot in FindStackableInventorySlots(needle)) {
                    byte toStack = (byte)((stackSlot.Item == null) ? needle.StackSize : (stackSlot.Item!.StackSize - stackSlot.Item!.Count));
                    toStack = (byte)MathF.Min(toStack, needle.Count);
                    foreach (var pickupClick in PickUpFromSlot(slot, toStack))
                        yield return pickupClick;
                    foreach (var putdownClick in PutDownToSlot(stackSlot, toStack))
                        yield return putdownClick;

                    left -= toStack;
                }

                if (left == 0) 
                    break;
            }

            if (count != null && left > 0 && throwIfNotEnough)
                throw new Exception("not enough items found");
        }


        public IEnumerable<WindowClick> PutItem(int itemId, byte? count = null, fNbt.NbtCompound? metadata = null)
        {
            if (this.InventoryWindow == null)
                throw new InvalidOperationException("Window must be extended with inventory window");

            var searchedItems = this.GetAllSlots().Skip(InventoryStart).Where(x => x.Item != null && x.Item!.Id == itemId);
            if (metadata != null)
                searchedItems = searchedItems.Where(x => x.Item!.Metadata == metadata);

            byte left = count ?? byte.MaxValue;

            foreach (var slot in searchedItems)
            {
                byte max = Math.Min(left, slot.Item!.Count);
                var needle = ItemFactory.CreateItem(itemId, max, slot.Item!.Damage, slot.Item!.Metadata);

                foreach (var stackSlot in FindStackableContainerSlots(needle)) 
                {
                    byte toStack = (byte)((stackSlot.Item == null) ? needle.StackSize : (stackSlot.Item!.StackSize - stackSlot.Item!.Count));
                    toStack = (byte)MathF.Min(toStack, needle.Count);
                    foreach (var pickupClick in PickUpFromSlot(slot, toStack))
                        yield return pickupClick;
                    foreach (var putdownClick in PutDownToSlot(stackSlot, toStack))
                        yield return putdownClick;

                    left -= toStack;
                }

                if (left == 0)
                    break;
            }
        }

        /// <summary>
        /// Returns window clicks to pick up <paramref name="count"/> items from the slot.
        /// <c>Window.SelectedSlot</c> must be empty
        /// </summary>
        /// <param name="slot">Slot</param>
        /// <param name="count">Number of items to pick up</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown when SelectedSlot is not empty</exception>
        /// <exception cref="ArgumentNullException">Thrown when slot.Item is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when count > slot.Item.Count </exception>
        public IEnumerable<WindowClick> PickUpFromSlot(Slot slot, byte count)
        {
            if (!SelectedSlot.IsEmpty())
                throw new InvalidOperationException("No item must be selected!");
            
            if (slot.Item == null) 
                throw new ArgumentNullException(nameof(slot.Item));

            if (slot.Item.Count < count) 
                throw new ArgumentOutOfRangeException(nameof(count));

            if (count == slot.Item.Count)
            {
                yield return new WindowClick(WindowOperationMode.SimpleClick, 0, slot.SlotNumber);
                yield break;
            }

            byte pickedUp = 0;
            if (count <= MathF.Ceiling(slot.Item.Count / 2.0f))
            {
                // pickup half stack
                yield return new WindowClick(WindowOperationMode.SimpleClick, 1, slot.SlotNumber);
                pickedUp = (byte)MathF.Ceiling(slot.Item.Count / 2.0f);
            }
            else
            {
                // else pickup full stack
                yield return new WindowClick(WindowOperationMode.SimpleClick, 0, slot.SlotNumber);
                pickedUp = slot.Item!.Count;
            }

            for (int i = 0; i < (pickedUp - count); i++)
            {
                // Put back redundant items
                yield return new WindowClick(WindowOperationMode.SimpleClick, 1, slot.SlotNumber);
            }
        }

        public IEnumerable<WindowClick> PutDownToSlot(Slot slot, byte count)
        {
            if (SelectedSlot!.IsEmpty())
                throw new InvalidOperationException("no item selected");

            if (SelectedSlot.Item!.Count < count)
                throw new ArgumentOutOfRangeException("count");

            if (SelectedSlot.Item!.Count <= count)
            {
                yield return new WindowClick(WindowOperationMode.SimpleClick, 0, slot.SlotNumber);
                yield break;
            }

            for (int i = 0; i < count; i++)
                yield return new WindowClick(WindowOperationMode.SimpleClick, 1, slot.SlotNumber);
        }
    }
}
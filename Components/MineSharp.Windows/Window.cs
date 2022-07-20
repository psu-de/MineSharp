using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Windows;

namespace MineSharp.Windows {
    public class Window : IDisposable {

        //TODO: Window Slot events

        public const int InventorySlotCount = 27;
        public const int HotbarSlotCount = 9;

        private static Logger Logger = Logger.GetLogger();

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
        private Slot? OffHandSlot { get; 
            set; }

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

            if (OffHandSlot?.SlotNumber == -1)
            {

            }
            
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
            if (OffHandSlot != null && OffHandSlot.SlotNumber == index) { 
                return OffHandSlot;
            }

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

        public void SwitchSlots(short slot1, short slot2) {
            Slot a = GetSlot(slot1);
            Slot b = GetSlot(slot2);

            if (a.IsEmpty() && b.IsEmpty())
                return;

            var click = new WindowClick(WindowOperationMode.SimpleClick, (byte)WindowMouseButton.MouseLeft, a.IsEmpty() ? slot2 : slot1);
            this.PerformClick(click);

            click = new WindowClick(WindowOperationMode.SimpleClick, (byte)WindowMouseButton.MouseLeft, a.IsEmpty() ? slot1 : slot2);
            this.PerformClick(click);
        }
    }
}
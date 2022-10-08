using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Windows;

namespace MineSharp.Windows
{
    public class Window : IDisposable
    {
        public delegate void WindowClickedEvent(Window sender, WindowClick click);

        public delegate void WindowClosedEvent(Window window);
        public delegate void WindowSlotEvent(Window sender, int slotIndex);

        //TODO: Window Slot events

        public const int InventorySlotCount = 27;
        public const int HotbarSlotCount = 9;

        private static Logger Logger = Logger.GetLogger();


        public Window(WindowInfo info, Slot[]? slots = null, Window? playerInventory = null)
        {
            this.Info = info;
            this.InventoryWindow = playerInventory;

            if (slots != null && slots.Length != info.UniqueSlots)
                throw new ArgumentException($"Invalid slot count ({slots.Length}) for window ({this.Info.Name}). Expected {this.TotalSlotCount}");

            if (slots == null)
            {
                this.ContainerSlots = new Slot[info.UniqueSlots];
                for (var i = 0; i < info.UniqueSlots; i++)
                {
                    this.ContainerSlots[i] = new Slot(null, (short)i);
                }
            } else this.ContainerSlots = slots;

            if (info.HasOffHandSlot) this.OffHandSlot = new Slot(null, (short)(this.TotalSlotCount - 1));
        }

        public Window(int windowId, WindowInfo info, Slot[]? slots = null, int? stateId = null, Window? playerInventory = null) : this(info, slots, playerInventory)
        {
            this.Id = windowId;

            this.StateId = stateId ?? 0;
            this.SelectedSlot = new Slot(null, -1);
        }

        public WindowInfo Info {
            get;
        }

        public int ContainerStart => 0;
        public int ContainerEnd => this.Info.UniqueSlots - 1;
        public int InventoryStart => this.Info.ExcludeInventory ? throw new NotSupportedException() : this.Info.UniqueSlots;
        public int InventoryEnd => this.InventoryStart + InventorySlotCount - 1;
        public int HotbarStart => this.InventoryEnd + 1;
        public int HotbarEnd => this.HotbarStart + HotbarSlotCount - 1;
        public int TotalSlotCount => (this.Info.ExcludeInventory ? this.Info.UniqueSlots : this.HotbarEnd + 1) + (this.Info.HasOffHandSlot ? 1 : 0);


        public int Id {
            get;
        }
        private Slot[] ContainerSlots { get; set; }
        private Slot? OffHandSlot {
            get;
            set;
        }

        public Window? InventoryWindow { get; private set; }

        public Slot? SelectedSlot { get; set; }
        public int StateId { get; set; }

        public int EmptySlotCount => this.EmptyContainerSlots().Length;
        public int AllEmptySlotCount => this.AllEmptySlots().Length;

        public void Dispose()
        {
            this.ContainerSlots = Array.Empty<Slot>();
            this.InventoryWindow = null;
        }

        /// <summary>
        ///     This event fires when the window has already been disposed.
        /// </summary>
        public event WindowClosedEvent? WindowClosed;
        public event WindowClickedEvent? WindowClicked;
        public event WindowSlotEvent? WindowSlotUpdated;

        internal void SwapSelectedSlot(int slotNumber)
        {
            var t = this.SelectedSlot!;
            this.SelectedSlot = this.GetSlot(slotNumber).Clone();
            this.SelectedSlot.SlotNumber = -1;

            t.SlotNumber = (short)slotNumber;
            this.SetSlot(t);
        }

        public void PerformClick(WindowClick click)
        {
            click.PerformClick(this);
            this.WindowClicked?.Invoke(this, click);
        }

        public void SetSlot(Slot slot)
        {

            if (this.OffHandSlot?.SlotNumber == -1) {}

            if (this.OffHandSlot != null && this.OffHandSlot.SlotNumber == slot.SlotNumber)
            {
                this.OffHandSlot = slot;
                this.WindowSlotUpdated?.Invoke(this, this.OffHandSlot.SlotNumber!);
                return;
            }

            if (slot.SlotNumber! >= this.ContainerSlots.Length)
            {
                if (this.Info.ExcludeInventory || this.InventoryWindow == null) throw new ArgumentOutOfRangeException("SlotNumber out of range");

                slot.SlotNumber -= (short)this.ContainerSlots.Length;
                this.InventoryWindow.SetSlot(slot);
                return;
            }

            this.ContainerSlots[slot.SlotNumber!] = slot;
            this.WindowSlotUpdated?.Invoke(this, slot.SlotNumber!);
        }

        public Slot GetSlot(int index)
        {
            if (this.OffHandSlot != null && this.OffHandSlot.SlotNumber == index)
            {
                return this.OffHandSlot;
            }

            if (index >= this.ContainerSlots.Length)
            {
                if (this.Info.ExcludeInventory || this.InventoryWindow == null) throw new ArgumentOutOfRangeException("index out of range");

                index -= (short)this.ContainerSlots.Length;
                return this.InventoryWindow.GetSlot(index);
            }

            return this.ContainerSlots[index];
        }

        public void UpdateSlots(Slot[] slotData)
        {
            foreach (var slot in slotData) this.SetSlot(slot);
        }

        public Slot[] GetContainerSlots()
        {
            var slots = new List<Slot>();
            slots.AddRange(this.ContainerSlots);

            if (this.OffHandSlot != null)
                slots.Add(this.OffHandSlot);
            return slots.ToArray();
        }

        public Slot[] GetAllSlots()
        {
            var slots = this.GetContainerSlots().ToList();

            if (this.Info.ExcludeInventory || this.InventoryWindow != null)
            {
                slots.AddRange(this.InventoryWindow!.GetContainerSlots().Select(x => new Slot(x.Item, (short)(x.SlotNumber! + this.ContainerSlots.Length))));
            }

            return slots.ToArray();
        }

        public Item?[] ContainerItems() => this.GetContainerSlots().Select(x => x.Item).ToArray();

        public Item?[] AllItems() => this.GetAllSlots().Select(x => x.Item).ToArray();

        /// <summary>
        ///     When using MineSharp.Bot, please use Bot.CloseWindow(windowId) instead.
        /// </summary>
        public void Close()
        {
            this.Dispose();
            this.WindowClosed?.Invoke(this);
        }

        public Slot[] EmptyContainerSlots()
        {
            var emptySlots = new List<Slot>();
            emptySlots.AddRange(this.ContainerSlots.Where(x => x.IsEmpty()));
            if (this.OffHandSlot != null && this.OffHandSlot.IsEmpty())
                emptySlots.Add(this.OffHandSlot);
            return emptySlots.ToArray();
        }

        public Slot[] AllEmptySlots()
        {
            var emptySlots = this.EmptyContainerSlots().ToList();
            if (!this.Info.ExcludeInventory && this.InventoryWindow != null)
            {
                emptySlots.AddRange(this.InventoryWindow.EmptyContainerSlots());
            }
            return emptySlots.ToArray();
        }


        private Slot? FindItem(Slot[] slots, Item searched) => slots.FirstOrDefault(x => !x.IsEmpty() && x.Item!.Id == searched.Id);

        /// <summary>
        ///     Searches through the container slots for an item
        /// </summary>
        /// <param name="itemInfo"></param>
        /// <returns></returns>
        public Slot? FindContainerItem(Item searched) => this.FindItem(this.ContainerSlots, searched);

        public Slot? FindInventoryItem(Item searched) => this.FindItem(this.InventoryWindow!.ContainerSlots, searched);

        public void SwitchSlots(short slot1, short slot2)
        {
            var a = this.GetSlot(slot1);
            var b = this.GetSlot(slot2);

            if (a.IsEmpty() && b.IsEmpty())
                return;

            var click = new WindowClick(WindowOperationMode.SimpleClick, (byte)WindowMouseButton.MouseLeft, a.IsEmpty() ? slot2 : slot1);
            this.PerformClick(click);

            click = new WindowClick(WindowOperationMode.SimpleClick, (byte)WindowMouseButton.MouseLeft, a.IsEmpty() ? slot1 : slot2);
            this.PerformClick(click);
        }
    }
}

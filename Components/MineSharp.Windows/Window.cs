
using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data;
using MineSharp.Windows.Clicks;

namespace MineSharp.Windows
{
    public class Window
    {
        private static readonly Logger Logger = Logger.GetLogger();
        
        public delegate void SlotChanged(Window window, short index);
        public delegate Task WindowSynchronizer(Window window, WindowClick click);
        
        
        public byte WindowId { get; }
        public int StateId { get; set; }
        public int WindowType { get; }
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
        
        public Window(byte id, int type, string title, int uniqueSlotCount, Window? inventory = null, WindowSynchronizer? windowSynchronizer = null)
        {
            this.WindowId = id;
            this.Title = title;
            this.WindowType = type;
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
        }

        /// <summary>
        /// This method sets the Windows slots to the new slot data, without considering the synchronisation of the window
        /// It should only be used when the server sends a <see cref="MineSharp.Data.Protocol.Play.Clientbound.PacketWindowItems"/> packet
        /// </summary>
        /// <param name="slots"></param>
        /// <exception cref="ArgumentException"></exception>
        public void SetProtocolSlots(Data.Protocol.Slot[] slots)
        {
            if (slots.Length != this.TotalSlotCount)
            {
                throw new ArgumentException($"slots length must match current windows slots length, got={slots.Length}, expected={this.TotalSlotCount}, id={this.WindowId}");
            }

            for (int i = 0; i < slots.Length; i++)
            {
                var slot = slots[i].ToSlot();
                slot.SlotNumber = (short)i;
                this.SetSlot(slot);
            }
        }
        
        /// <summary>
        /// This method set a single Window slot to the new slot data, without considering the synchronisation of the window
        /// It should only be used when the server sends a <see cref="MineSharp.Data.Protocol.Play.Clientbound.PacketSetSlot"/> packet
        /// </summary>
        public void SetProtocolSlot(Data.Protocol.Slot slot, short index)
        {
            ThrowIfSlotOufOfRange(index);
            
            var windowSlot = slot.ToSlot();
            windowSlot.SlotNumber = index;
            this.SetSlot(windowSlot);
        }

        /// <summary>
        /// Sets the selected slot
        /// </summary>
        /// <param name="slot"></param>
        public void SetSelectedSlot(Slot slot)
        {
            if (this.Inventory != null)
            {
                this.Inventory.SetSelectedSlot(slot);
            } else
            {
                slot.SlotNumber = -1;
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
            } else
            {
                return this._selectedSlot!;
            }
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
                slots.AddRange(this.Inventory.Slots.Select(x => new Slot(x.Item, (short)(x.SlotNumber + this.Slots.Length))));
            }
            
            if (this.OffhandSlot != null)
            {
                slots.Add(this.OffhandSlot);
            }
            
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
            {   
                return this.GetSelectedSlot().Clone();
            } else if (index == 45 && this.OffhandSlot != null)
            {
                return this.OffhandSlot;
            } else if (index >= this.Slots.Length)
            {
                return this.Inventory!.GetSlot((short)(index - this.Slots.Length));
            } else
            {
                return this.Slots[index].Clone();
            }
        }
        
        /// <summary>
        /// Sets the slot at the given index
        /// </summary>
        /// <param name="slot"></param>
        internal void SetSlot(Slot slot)
        {
            ThrowIfSlotOufOfRange(slot.SlotNumber);
            
            if (slot.SlotNumber == -1)
            {
                this.SetSelectedSlot(slot);
            } else if (slot.SlotNumber == 45 && this.OffhandSlot != null)
            {
                this.OffhandSlot = slot;
            } else if (slot.SlotNumber < this.Slots.Length)
            {
                this.Slots[slot.SlotNumber] = slot;
            } else
            {
                slot.SlotNumber -= (short)this.Slots.Length;
                this.Inventory!.SetSlot(slot);
                return;
            }
            OnSlotChanged?.Invoke(this, slot.SlotNumber);
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

        public void DoSimpleClick(WindowMouseButton button, short clickedSlot)
        {
            ThrowIfSlotOufOfRange(clickedSlot);
            if (button == WindowMouseButton.MouseMiddle)
            {
                throw new ArgumentException($"{nameof(SimpleWindowClick)} does not support mouse button {nameof(WindowMouseButton.MouseMiddle)}");
            }

            var click = new SimpleWindowClick(this, clickedSlot, (byte)button);
            this.PerformWindowClick(click);
        }

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
}

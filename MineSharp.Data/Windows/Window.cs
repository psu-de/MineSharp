using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Items;

namespace MineSharp.Data.Windows {
    public abstract class Window {
        private static Logger Logger = Logger.GetLogger();

        public delegate void EmptyEvent(Window sender);
        public delegate void ItemEvent(Window sender, Item? item);
        public delegate void ClickEvent(Window sender, short slotIndex, WindowOperationMode mode, byte button);


        public event ItemEvent SelectedItemChanged;

        /// <summary>
        /// Fires whenever the Inventory was updated
        /// </summary>
        public event EmptyEvent WindowUpdated;

        /// <summary>
        /// Fires when the an Click was performed on the window
        /// </summary>
        public event ClickEvent Clicked;

        public abstract byte WindowType { get; }
        public abstract string Title { get; }
        public virtual int[] DisabledShiftSlots => new int[0];
        public virtual bool IsPlayerInventory => false;
        public abstract int InventoryStart { get; }
        public int HotbarStart => InventoryStart + (3 * 9);

        public int StateId { get; protected set; }
        public Dictionary<int, WindowSlot> Slots { get; protected set; } = new Dictionary<int, WindowSlot>();
        public int SlotCount => Slots.Count;


        private Item? _selectedItem = null;

        public Item? SelectedItem { get => _selectedItem; protected set { if (_selectedItem == value) return; _selectedItem = value; SelectedItemChanged?.Invoke(this, value); } }

        public Window(int slotCount) {
            for (int i = 0; i < slotCount; i++) {
                Slots.Add(i, new WindowSlot(null, i, GetAllowedItemsForSlot(i), DisabledShiftSlots.Contains(i)));
            }
        }

        public static Window CreateWindowById(int id) {
            switch (id) {
                case 2: return new ChestWindow();
                case 5: return new LargeChestWindow();
                default: throw new NotImplementedException();
            }
        }


        protected abstract ItemInfo[]? GetAllowedItemsForSlot(int slotIndex);

        public WindowSlot[] GetHotbarSlots() {

            WindowSlot[] slots = new WindowSlot[9];

            for (int i = 0; i < 9; i++) {
                slots[i] = Slots[i + HotbarStart];
            }

            return slots;
        }

        public void SetSlot(int index, Slot? slot) {
            if (index < 0 || index >= this.SlotCount) throw new IndexOutOfRangeException();

            Item? item = (slot == null) ? null : new Item(ItemData.Items[slot.ItemID], slot.Count, slot.ItemDamage, slot.Nbt);
            Slots[index].Item = item;
            WindowUpdated?.Invoke(this);
        }

        public void UpdateSlots(Slot?[] slots) {

            if (SlotCount != slots.Length) throw new ArgumentException($"Slots array must be same length as SlotCount (Got: {slots.Length}, Expected={SlotCount}");

            for (int i = 0; i < slots.Length; i++) {
                var item = slots[i] == null ? null : new Item(ItemData.Items[slots[i].ItemID], slots[i].Count, slots[i].ItemDamage, slots[i].Nbt);
                Slots[i].Item = item;
            }

            WindowUpdated?.Invoke(this);
        }


        public void Click(short slotIndex, WindowOperationMode mode, byte button) {
            switch (mode) {
                case WindowOperationMode.MouseLeftRight: handleMouseClick(slotIndex, button); break;
                case WindowOperationMode.ShiftMouseLeftRight: handleShiftMouseClick(slotIndex, button); break;
                case WindowOperationMode.Hotkey: handleHotkeyClick(slotIndex, button); break;
                case WindowOperationMode.DropKey: handleDropKeyClick(slotIndex, button); break;
                default: throw new NotImplementedException();
            }

            Clicked?.Invoke(this, slotIndex, mode, button);
        }

        private void handleDropKeyClick(short slotIndex, byte button) {
            if (this.SelectedItem != null) throw new NotSupportedException();

            if (Slots[slotIndex].Item == null) return;

            if (button == 0) {
                Item? a = Slots[slotIndex].Item;
                a.Count -= 1;
                if (a.Count <= 0) a = null;

                Slots[slotIndex].Item = a;
            } else {
                Slots[slotIndex].Item = null;
            }
        }

        private void handleOutsideMouseClick(short slotIndex, byte button) {
            if (button == 0) {   // Left mouse button

                if (this.SelectedItem == null) return;

                this.SelectedItem = null; // Stack 'dropped'

                return;
            } else if (button == 1) { // Right mouse button

                if (this.SelectedItem == null) return;
                this.SelectedItem.Count -= 1; // Drop one item

                if (this.SelectedItem.Count == 0) { this.SelectedItem = null; return; } // No item left
                this.SelectedItemChanged?.Invoke(this, this.SelectedItem);
                return;
            } else throw new NotSupportedException();
        }

        private void handleMouseClick(short slotIndex, byte button) {
            if (slotIndex == -999) { // Outside click
                handleOutsideMouseClick(slotIndex, button);
                return;
            }

            if (button == 0) { // Left mouse button
                if (this.SelectedItem == null) { // Pick up stack from inventory 
                    WindowSlot? pickUpSlot = this.Slots[slotIndex];

                    if (pickUpSlot.IsEmpty) return;

                    this.SelectedItem = pickUpSlot.Item;
                    this.Slots[slotIndex].Item = null;

                    WindowUpdated?.Invoke(this);

                } else { // Put down stack

                    WindowSlot putDownSlot = this.Slots[slotIndex];

                    if (putDownSlot.PutDown(this.SelectedItem, out byte itemsTaken)) {
                        this.SelectedItem = null;
                    } else {
                        this.SelectedItem.Count -= itemsTaken;
                        SelectedItemChanged.Invoke(this, this.SelectedItem);
                    }

                    WindowUpdated?.Invoke(this);
                    return;
                }
            } else { // Right mouse button
                if (this.SelectedItem == null) { // Pick up half stack
                    WindowSlot pickUpSlot = this.Slots[slotIndex];

                    if (pickUpSlot.IsEmpty) return;

                    Item halfStack = new Item(pickUpSlot.Item.Info, 0, pickUpSlot.Item.Damage, pickUpSlot.Item.Metadata);

                    pickUpSlot.Take(ref halfStack, (byte)Math.Ceiling(pickUpSlot.Item.Count / 2f));
                    this.SelectedItem = halfStack;

                    WindowUpdated?.Invoke(this);
                } else { // Put down one item

                    if (this.Slots[slotIndex].PutDown(this.SelectedItem, out byte itemsTaken, 1)) {
                        this.SelectedItem = null;

                    } else if (itemsTaken <= 0) { // Swap items
                        Item temp = this.SelectedItem;
                        this.SelectedItem = this.Slots[slotIndex].Item;
                        this.Slots[slotIndex].Item = temp;
                    } else {
                        this.SelectedItem.Count -= 1;
                        SelectedItemChanged?.Invoke(this, this.SelectedItem);
                    }
                }

                WindowUpdated?.Invoke(this);
            }
        }

        private void handleShiftMouseClick(short slotIndex, byte button) {
            if (button == 0) { // Shift left click
                if (Slots[slotIndex].IsEmpty) return;


                Item temp = new Item(this.Slots[slotIndex].Item.Info, 0, this.Slots[slotIndex].Item.Damage, this.Slots[slotIndex].Item.Metadata);

                if (slotIndex >= InventoryStart) {

                    int start = -1;
                    int end = -1;
                    if (this.IsPlayerInventory) {

                        if (slotIndex >= HotbarStart) {
                            start = 0;
                            end = this.HotbarStart;
                        } else {
                            int _idx = -1;
                            while (temp.Count > 0 && (_idx = FindNextSlotToFillUp(temp, 0, this.InventoryStart, true)) != -1) {
                                if (this.Slots[_idx].PutDown(temp, out byte taken)) {
                                    temp.Count = 0;
                                    break;
                                }

                                temp.Count -= taken;
                            }

                            while (temp.Count > 0 && (_idx = FindNextSlotToFillUp(temp, this.HotbarStart, this.HotbarStart + 9, true)) != -1) {
                                if (this.Slots[_idx].PutDown(temp, out byte taken)) {
                                    temp.Count = 0;
                                    break;
                                }

                                temp.Count -= taken;
                            }


                            if (temp.Count <= 0) Slots[slotIndex].Item = null;
                            else Slots[slotIndex].Item = temp;

                            WindowUpdated?.Invoke(this);
                            return;
                        }

                    } else {
                        start = 0;
                        end = this.InventoryStart;
                    }

                    int idx = -1;
                    while (temp.Count > 0 && (idx = FindNextSlotToFillUp(temp, start, end, true)) != -1) {
                        if (this.Slots[idx].PutDown(temp, out byte taken)) {
                            temp.Count = 0;
                            break;
                        }

                        temp.Count -= taken;
                    }

                    if (temp.Count <= 0) Slots[slotIndex].Item = null;
                    else Slots[slotIndex].Item = temp;

                    WindowUpdated?.Invoke(this);
                }
            }
        }

        private void handleHotkeyClick(short slotIndex, byte button) {
            if (button >= 0 && button <= 8) {
                SwapSlots(slotIndex, button);
                WindowUpdated?.Invoke(this);
                return;
            }

            if (button != 40) throw new NotSupportedException();

            SwapSlots(slotIndex, (short)(InventoryStart + (4 * 9))); // TODO: Checken ob des auch mit anderen windows funktioniert ausser inventory
        }


        private void SwapSlots(short a, short b) {
            Item? temp = this.Slots[a].Item;
            this.Slots[a].Item = this.Slots[b].Item;
            this.Slots[b].Item = temp;
        }

        private int FindNextSlotToFillUp(Item item, int start, int end, bool allowsShift) {
            int[] indicies = this.Slots.Where(x => x.Value.Item != null && x.Value.Item.Info.Id == item.Info.Id && (item.Info.StackSize - x.Value.Item.Count) > 0).Select(x => x.Key).ToArray();
            if (indicies.Length > 0) return indicies[0];

            return FindEmptySlot(start, end, allowsShift);
        }

        private int FindEmptySlot(int start, int end, bool allowsShift) {
            for (int i = start; i < end; i++) {
                if (this.Slots[i].IsEmpty && (!allowsShift || this.Slots[i].AllowsShift)) return i;
            }
            return -1;
        }

        public Slot[] GetSlotData() {
            return Slots.Select(x => { var s = x.Value?.ToSlot() ?? Slot.Empty; s.SlotNumber = (short)x.Key; return s; }).ToArray();
        }
    }
}
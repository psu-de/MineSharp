using MineSharp.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Windows {
    public class WindowSlot {

        public delegate void ItemEvent(WindowSlot sender, Item? newItem);
        public event ItemEvent ItemChanged;

        private Item? _item;
        public Item? Item { get { return _item; } set { if (_item == value) return; _item = value; ItemChanged?.Invoke(this, value); } }
        public int Slot { get; set; }
        public ItemInfo[]? AllowedItems { get; set; }
        public bool IsEmpty => Item == null;
        public bool AllowsShift { get; set; }




        public WindowSlot(Item? item, int slot, ItemInfo[]? allowedItems, bool allowsShift) {
            this.Item = item;
            this.Slot = slot;
            this.AllowedItems = allowedItems;
            this.AllowsShift = allowsShift;
        }



        public bool IsAllowed(Item item) => AllowedItems?.Contains(item.Info) ?? true;

        /// <summary>
        /// Puts down an item stack on a window slot
        /// </summary>
        /// <param name="item">Item to put down</param>
        /// <param name="itemsTaken">How many items where taken from the stack</param>
        /// <param name="max">Max amount to take</param>
        /// <returns>Returns <see cref="true"/> when all items from <paramref name="item"/> where taken</returns>
        public bool PutDown(Item item, out byte itemsTaken, byte? max = null) {

            if (!IsAllowed(item)) { itemsTaken = 0; return false; }

            if (this.Item == null) {
                itemsTaken = Math.Min(item.Count, max ?? byte.MaxValue);
                this.Item = item;
                this.Item.Count = itemsTaken;

                return item.Count - itemsTaken == 0;
            }

            if (this.Item.Info.Id != item.Info.Id) { itemsTaken = 0; return false; }

            byte toStore = (byte)(this.Item.Info.StackSize - this.Item.Count);
            if (toStore > 0) {
                byte stored = Math.Min(toStore, Math.Min(item.Count, max ?? byte.MaxValue));

                this.Item.Count += stored;
                ItemChanged?.Invoke(this, this.Item);

                itemsTaken = stored;
                return item.Count - stored <= 0;
            } else {
                itemsTaken = 0;
                return false;
            }
        }

        public int Take(ref Item receiver, byte? max = null) {
            if (Item == null || receiver.Info.Id != Item.Info.Id) {
                return 0;
            }

            byte maxPossible = Math.Min((byte)(receiver.Info.StackSize - receiver.Count), Math.Min(this.Item.Count, max ?? byte.MaxValue));

            if (maxPossible <= 0) {
                return 0;
            }

            this.Item.Count -= maxPossible;
            if (this.Item.Count <= 0) this.Item = null;
            else ItemChanged?.Invoke(this, this.Item);

            receiver.Count += maxPossible;
            return maxPossible;
        }

        public Core.Types.Slot ToSlot() => this.Item?.ToSlot() ?? Core.Types.Slot.Empty;
    }
}

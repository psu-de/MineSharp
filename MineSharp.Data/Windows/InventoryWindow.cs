using MineSharp.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Windows {
    public class InventoryWindow : Window {
        public override byte WindowType => 0;

        public override string Title => "Inventory";

        public override bool IsPlayerInventory => true;
        public override int[] DisabledShiftSlots => new[] { 1, 2, 3, 4, 45 }; // Disabled slots: Crafting Inputs + OffHand
        public override int InventoryStart => (int)InvSlots.MainInventoryStart;

        public InventoryWindow() : base(46) { }


        public int GetHotbarSlotIndex(int hotbarIndex) => (int)InvSlots.HotBarStart + hotbarIndex;

        /// <summary>
        /// Gets the item at the hotbar slot index (0-8)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Item? GetHotbarSlot(int index) {

            if (index < 0 || index > 8) throw new IndexOutOfRangeException();
            return this.Slots[GetHotbarSlotIndex(index)].Item;
        }

        protected override ItemInfo[]? GetAllowedItemsForSlot(int slotIndex) {
            switch (slotIndex) {

                case (int)InvSlots.CraftingOutput: return new ItemInfo[0]; // No item is allowed to be placed
                case (int)InvSlots.ArmorHead: return ItemData.Items.Where(x => x.Name.EndsWith("helmet") || x.Id == ItemType.TurtleShell || x.Id == ItemType.CarvedPumpkin).ToArray();
                case (int)InvSlots.ArmorChest: return ItemData.Items.Where(x => x.Name.EndsWith("chestplate")).ToArray();
                case (int)InvSlots.ArmorLegs: return ItemData.Items.Where(x => x.Name.EndsWith("leggings")).ToArray();
                case (int)InvSlots.ArmorFeet: return ItemData.Items.Where(x => x.Name.EndsWith("boots")).ToArray();

                default:
                    return null;
            }
        }

        public enum InvSlots {
            CraftingOutput = 0,
            CraftingInputStart = 1,
            ArmorHead = 5,
            ArmorChest = 6,
            ArmorLegs = 7,
            ArmorFeet = 8,
            MainInventoryStart = 9,
            HotBarStart = 36,
            OffhandSlot = 45
        }
    }
}

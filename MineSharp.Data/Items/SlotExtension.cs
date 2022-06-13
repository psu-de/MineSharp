using MineSharp.Core.Types;

namespace MineSharp.Data.Items {
    public static class SlotExtension {


        public static ItemInfo? GetItemInfo(this Slot slot) => slot.ItemID == -1 ? null : ItemData.Items[slot.ItemID];
        public static bool CanStack(this Slot slot, Slot otherSlot) {
            var slotType = slot.ItemID;
            var otherSlotType = otherSlot.ItemID;

            if (slotType == otherSlotType) {

                var itemInfo = ItemData.Items[slotType];

                if (itemInfo.StackSize == 1) return false;

                return itemInfo.StackSize - slot.Count > 0;

            } else return false;
        }

        public static Item? AsItem(this Slot slot) {
            var info = slot.GetItemInfo();
            if (info == null) return null;
            return new Item(info, slot.Count, slot.ItemDamage, slot.Nbt);
        }

    }
}

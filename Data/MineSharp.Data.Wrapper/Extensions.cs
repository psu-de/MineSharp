using MineSharp.Data.Items;

namespace MineSharp.Data {
    public static class SlotExtensions {
        public static MineSharp.Data.Protocol.Slot ToProtocolSlot(this Core.Types.Slot slot) {
            return new Data.Protocol.Slot(!slot.IsEmpty(), new Data.Protocol.Slot.AnonSwitch(slot.IsEmpty() ? null : new Data.Protocol.Slot.AnonSwitch.AnonSwitchStatetrueContainer(slot.Item!.Id, (sbyte)slot.Item!.Count, slot.Item!.Metadata)));
        }

        public static MineSharp.Core.Types.Slot ToSlot(this Data.Protocol.Slot slot) {

            if (slot.Present) {
                var anon = (Protocol.Slot.AnonSwitch.AnonSwitchStatetrueContainer)slot.Anon!;
                return new Core.Types.Slot(ItemFactory.CreateItem(anon.ItemId, (byte)anon.ItemCount, null, anon.NbtData), -1);
            }
            return new Core.Types.Slot(null, -1);

        }
    }

    public static class PositionExtensions {
        public static MineSharp.Data.Protocol.PositionBitfield ToProtocolPosition(this Core.Types.Position pos) {
            return new Data.Protocol.PositionBitfield(pos.ToULong());
        }
    }
}
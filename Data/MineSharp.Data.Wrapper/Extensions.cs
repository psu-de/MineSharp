using MineSharp.Data.Items;

namespace MineSharp.Data
{
    public static class SlotExtensions
    {
        public static Protocol.Slot ToProtocolSlot(this Core.Types.Slot slot) => new Protocol.Slot(!slot.IsEmpty(), new Protocol.Slot.AnonSwitch(slot.IsEmpty() ? null : new Protocol.Slot.AnonSwitch.AnonSwitchStatetrueContainer(slot.Item!.Id, (sbyte)slot.Item!.Count, slot.Item!.Metadata)));

        public static Core.Types.Slot ToSlot(this Protocol.Slot slot)
        {

            if (slot.Present)
            {
                var anon = (Protocol.Slot.AnonSwitch.AnonSwitchStatetrueContainer)slot.Anon!;
                return new Core.Types.Slot(ItemFactory.CreateItem(anon.ItemId, (byte)anon.ItemCount, null, anon.NbtData), -1);
            }
            return new Core.Types.Slot(null, -1);

        }
    }

    public static class PositionExtensions
    {
        public static Protocol.PositionBitfield ToProtocolPosition(this Core.Types.Position pos) => new Protocol.PositionBitfield(pos.ToULong());
    }
}

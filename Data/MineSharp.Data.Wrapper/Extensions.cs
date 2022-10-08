using MineSharp.Core.Types;
using MineSharp.Data.Items;
using MineSharp.Data.Protocol;
using Slot = MineSharp.Data.Protocol.Slot;
namespace MineSharp.Data
{
    public static class SlotExtensions
    {
        public static Slot ToProtocolSlot(this Core.Types.Slot slot) => new Slot(!slot.IsEmpty(), new Slot.AnonSwitch(slot.IsEmpty() ? null : new Slot.AnonSwitch.AnonSwitchStatetrueContainer(slot.Item!.Id, (sbyte)slot.Item!.Count, slot.Item!.Metadata)));

        public static Core.Types.Slot ToSlot(this Slot slot)
        {

            if (slot.Present)
            {
                var anon = (Slot.AnonSwitch.AnonSwitchStatetrueContainer)slot.Anon!;
                return new Core.Types.Slot(ItemFactory.CreateItem(anon.ItemId, (byte)anon.ItemCount, null, anon.NbtData), -1);
            }
            return new Core.Types.Slot(null, -1);

        }
    }

    public static class PositionExtensions
    {
        public static PositionBitfield ToProtocolPosition(this Position pos) => new PositionBitfield(pos.ToULong());
    }
}

using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;

namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class ClickWindowPacket : Packet {

        public byte WindowID { get; private set; }
        public int StateID { get; private set; }
        public short Slot { get; private set; }
        public byte Button { get; private set; }
        public WindowOperationMode Mode { get; private set; }
        public Slot[] SlotData { get; private set; }
        public Slot ClickedItem { get; private set; }

        public ClickWindowPacket() { }

        public ClickWindowPacket(byte windowid, int stateid, short slot, byte button, WindowOperationMode mode, Slot[] slotdata, Slot clickeditem) {
            this.WindowID = windowid;
            this.StateID = stateid;
            this.Slot = slot;
            this.Button = button;
            this.Mode = mode;
            this.SlotData = slotdata;
            this.ClickedItem = clickeditem;
        }

        public override void Read(PacketBuffer buffer) {
            throw new NotImplementedException();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteByte(this.WindowID);
            buffer.WriteVarInt(this.StateID);
            buffer.WriteShort(this.Slot);
            buffer.WriteByte(this.Button);
            buffer.WriteVarInt((int)this.Mode);
            buffer.WriteSlotArray(this.SlotData);
            buffer.WriteSlot(this.ClickedItem);
        }
    }
}
using MineSharp.Core.Types;
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class SetSlotPacket : Packet {

        public byte WindowID { get; private set; }
        public int StateID { get; private set; }
        public short Slot { get; private set; }
        public Slot? SlotData { get; private set; }

        public SetSlotPacket() { }

        public SetSlotPacket(byte windowid, int stateid, short slot, Slot slotdata) {
            this.WindowID = windowid;
            this.StateID = stateid;
            this.Slot = slot;
            this.SlotData = slotdata;
        }

        public override void Read(PacketBuffer buffer) {
            this.WindowID = buffer.ReadByte();
            this.StateID = buffer.ReadVarInt();
            this.Slot = buffer.ReadShort();
            this.SlotData = buffer.ReadSlot();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteByte(this.WindowID);
            buffer.WriteVarInt(this.StateID);
            buffer.WriteShort(this.Slot);
            buffer.WriteSlot(this.SlotData);
        }
    }
}
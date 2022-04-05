using MineSharp.Core.Types;
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class WindowItemsPacket : Packet {

        public byte WindowID { get; private set; }
        public int StateID { get; private set; }
        public Slot[] SlotData { get; private set; }
        public Slot CarriedItem { get; private set; }

        public WindowItemsPacket() { }

        public WindowItemsPacket(byte windowid, int stateid, Slot[] slotdata, Slot carrieditem) {
            this.WindowID = windowid;
            this.StateID = stateid;
            this.SlotData = slotdata;
            this.CarriedItem = carrieditem;
        }

        public override void Read(PacketBuffer buffer) {
            this.WindowID = buffer.ReadByte();
            this.StateID = buffer.ReadVarInt();
            this.SlotData = buffer.ReadSlotArray();
            this.CarriedItem = buffer.ReadSlot();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteByte(this.WindowID);
            buffer.WriteVarInt(this.StateID);
            buffer.WriteSlotArray(this.SlotData);
            buffer.WriteSlot(this.CarriedItem);
        }
    }
}
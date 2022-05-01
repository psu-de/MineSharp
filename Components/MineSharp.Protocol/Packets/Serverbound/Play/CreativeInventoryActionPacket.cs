using MineSharp.Core.Types;

namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class CreativeInventoryActionPacket : Packet {

        public short Slot { get; private set; }
        public Slot ClickedItem { get; private set; }

        public CreativeInventoryActionPacket() { }

        public CreativeInventoryActionPacket(short slot, Slot clickeditem) {
            this.Slot = slot;
            this.ClickedItem = clickeditem;
        }

        public override void Read(PacketBuffer buffer) {
            this.Slot = buffer.ReadShort();
            this.ClickedItem = buffer.ReadSlot();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteShort(this.Slot);
            buffer.WriteSlot(this.ClickedItem);
        }
    }
}
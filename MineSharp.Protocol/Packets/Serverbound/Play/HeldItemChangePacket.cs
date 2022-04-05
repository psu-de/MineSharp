namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class HeldItemChangePacket : Packet {

        public short Slot { get; private set; }

        public HeldItemChangePacket() { }

        public HeldItemChangePacket(short slot) {
            this.Slot = slot;
        }

        public override void Read(PacketBuffer buffer) {
            this.Slot = buffer.ReadShort();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteShort(this.Slot);
        }
    }
}
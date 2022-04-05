namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class HeldItemChangePacket : Packet {

        public byte Slot { get; private set; }

        public HeldItemChangePacket() { }

        public HeldItemChangePacket(byte slot) {
            this.Slot = slot;
        }

        public override void Read(PacketBuffer buffer) {
            this.Slot = buffer.ReadByte();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteByte(this.Slot);
        }
    }
}
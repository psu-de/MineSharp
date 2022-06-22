namespace MineSharp.Protocol.Packets.Serverbound.Status {
    public class PingPacket : Packet {

        public long Payload { get; private set; }

        public PingPacket() { }

        public PingPacket(long payload) {
            this.Payload = payload;
        }

        public override void Read(PacketBuffer buffer) {
            this.Payload = buffer.ReadLong();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteLong(this.Payload);
        }
    }
}

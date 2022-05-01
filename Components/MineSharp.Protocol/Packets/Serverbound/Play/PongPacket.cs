namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class PongPacket : Packet {

        public int ID { get; private set; }

        public PongPacket() { }

        public PongPacket(int id) {
            this.ID = id;
        }

        public override void Read(PacketBuffer buffer) {
            this.ID = buffer.ReadInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteInt(this.ID);
        }
    }
}
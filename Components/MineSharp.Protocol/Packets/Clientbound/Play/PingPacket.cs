namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class PingPacket : Packet {

        public int ID { get; private set; }

        public PingPacket() { }

        public PingPacket(int id) {
            this.ID = id;
        }

        public override void Read(PacketBuffer buffer) {
            this.ID = buffer.ReadInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteInt(this.ID);
        }

        public override Task Sent(MinecraftClient client) {
            client.SendPacket(new Serverbound.Play.PongPacket(this.ID));
            return base.Sent(client);
        }
    }
}
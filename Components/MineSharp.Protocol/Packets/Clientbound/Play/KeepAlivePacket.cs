namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class KeepAlivePacket : Packet {

        public long KeepAliveID { get; private set; }

        public KeepAlivePacket() { }

        public KeepAlivePacket(long KeepAliveID) {
            this.KeepAliveID = KeepAliveID;
        }

        public override async Task Handle(MinecraftClient client) {
            client.SendPacket(new Serverbound.Play.KeepAlivePacket(this.KeepAliveID));
            await base.Handle(client);
        }

        public override void Read(PacketBuffer buffer) {
            this.KeepAliveID = buffer.ReadLong();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteLong(KeepAliveID);
        }
    }
}

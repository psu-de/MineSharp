namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class ResourcePackStatusPacket : Packet {

        public int /* TODO: Enum! */ Result { get; private set; }

        public ResourcePackStatusPacket() { }

        public ResourcePackStatusPacket(int /* TODO: Enum! */ result) {
            this.Result = result;
        }

        public override void Read(PacketBuffer buffer) {
            this.Result = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.Result);
        }
    }
}
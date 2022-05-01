namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class OpenBookPacket : Packet {

        public int /* TODO: Enum! */ Hand { get; private set; }

        public OpenBookPacket() { }

        public OpenBookPacket(int /* TODO: Enum! */ hand) {
            this.Hand = hand;
        }

        public override void Read(PacketBuffer buffer) {
            this.Hand = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.Hand);
        }
    }
}
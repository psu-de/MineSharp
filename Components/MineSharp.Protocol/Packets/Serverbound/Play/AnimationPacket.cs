namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class AnimationPacket : Packet {

        public int /* TODO: Enum! */ Hand { get; private set; }

        public AnimationPacket() { }

        public AnimationPacket(int /* TODO: Enum! */ hand) {
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
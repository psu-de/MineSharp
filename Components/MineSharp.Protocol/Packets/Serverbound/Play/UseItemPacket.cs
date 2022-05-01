namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class UseItemPacket : Packet {

        public int /* TODO: Enum! */ Hand { get; private set; }

        public UseItemPacket() { }

        public UseItemPacket(int /* TODO: Enum! */ hand) {
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
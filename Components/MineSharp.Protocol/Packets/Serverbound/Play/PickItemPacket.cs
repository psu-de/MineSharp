namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class PickItemPacket : Packet {

        public int Slottouse { get; private set; }

        public PickItemPacket() { }

        public PickItemPacket(int slottouse) {
            this.Slottouse = slottouse;
        }

        public override void Read(PacketBuffer buffer) {
            this.Slottouse = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.Slottouse);
        }
    }
}
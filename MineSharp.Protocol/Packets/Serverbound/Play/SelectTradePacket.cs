namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class SelectTradePacket : Packet {

        public int Selectedslot { get; private set; }

        public SelectTradePacket() { }

        public SelectTradePacket(int selectedslot) {
            this.Selectedslot = selectedslot;
        }

        public override void Read(PacketBuffer buffer) {
            this.Selectedslot = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.Selectedslot);
        }
    }
}
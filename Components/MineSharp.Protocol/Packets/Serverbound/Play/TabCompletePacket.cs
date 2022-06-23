namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class TabCompletePacket : Packet {

        public int TransactionId { get; private set; }
        public string? Text { get; private set; }

        public TabCompletePacket() { }

        public TabCompletePacket(int transactionid, string text) {
            this.TransactionId = transactionid;
            this.Text = text;
        }

        public override void Read(PacketBuffer buffer) {
            this.TransactionId = buffer.ReadVarInt();
            this.Text = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.TransactionId);
            buffer.WriteString(this.Text!);
        }
    }
}
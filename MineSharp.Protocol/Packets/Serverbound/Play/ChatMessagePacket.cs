namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class ChatMessagePacket : Packet {

        public string Message { get; private set; }

        public ChatMessagePacket() { }
        public ChatMessagePacket(string message) {
            Message = message;
        }

        public override void Read(PacketBuffer buffer) {
            this.Message = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString(this.Message);
        }
    }
}

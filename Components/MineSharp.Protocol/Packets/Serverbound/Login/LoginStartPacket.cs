namespace MineSharp.Protocol.Packets.Serverbound.Login {
    public class LoginStartPacket : Packet {

        public string? Username { get; private set; }

        public LoginStartPacket() { }
        public LoginStartPacket(string username) {
            this.Username = username;
        }

        public override void Read(PacketBuffer buffer) {
            this.Username = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString(Username!);
        }
    }
}

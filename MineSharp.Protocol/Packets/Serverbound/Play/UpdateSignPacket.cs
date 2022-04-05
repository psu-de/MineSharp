using MineSharp.Core.Types;

namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class UpdateSignPacket : Packet {

        public Position? Location { get; private set; }
        public string Line1 { get; private set; }
        public string Line2 { get; private set; }
        public string Line3 { get; private set; }
        public string Line4 { get; private set; }

        public UpdateSignPacket() { }

        public UpdateSignPacket(Position? location, string line1, string line2, string line3, string line4) {
            this.Location = location;
            this.Line1 = line1;
            this.Line2 = line2;
            this.Line3 = line3;
            this.Line4 = line4;
        }

        public override void Read(PacketBuffer buffer) {
            this.Location = buffer.ReadPosition();
            this.Line1 = buffer.ReadString();
            this.Line2 = buffer.ReadString();
            this.Line3 = buffer.ReadString();
            this.Line4 = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WritePosition(this.Location);
            buffer.WriteString(this.Line1);
            buffer.WriteString(this.Line2);
            buffer.WriteString(this.Line3);
            buffer.WriteString(this.Line4);
        }
    }
}
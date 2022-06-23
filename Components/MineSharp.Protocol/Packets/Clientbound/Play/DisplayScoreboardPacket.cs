namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class DisplayScoreboardPacket : Packet {

        public byte Position { get; private set; }
public string? ScoreName { get; private set; }

        public DisplayScoreboardPacket() { }

        public DisplayScoreboardPacket(byte position, string scorename) {
            this.Position = position;
this.ScoreName = scorename;
        }

        public override void Read(PacketBuffer buffer) {
            this.Position = buffer.ReadByte();
this.ScoreName = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteByte(this.Position);
buffer.WriteString(this.ScoreName!);
        }
    }
}
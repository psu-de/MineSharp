namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class ScoreboardObjectivePacket : Packet {

        public string ObjectiveName { get; private set; }
public byte Mode { get; private set; }

        public ScoreboardObjectivePacket() { }

        public ScoreboardObjectivePacket(string objectivename, byte mode) {
            this.ObjectiveName = objectivename;
this.Mode = mode;
        }

        public override void Read(PacketBuffer buffer) {
            this.ObjectiveName = buffer.ReadString();
this.Mode = buffer.ReadByte();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString(this.ObjectiveName);
buffer.WriteByte(this.Mode);
        }
    }
}
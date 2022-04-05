namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class UpdateScorePacket : Packet {

        public string EntityName { get; private set; }
public byte Action { get; private set; }
public string ObjectiveName { get; private set; }

        public UpdateScorePacket() { }

        public UpdateScorePacket(string entityname, byte action, string objectivename) {
            this.EntityName = entityname;
this.Action = action;
this.ObjectiveName = objectivename;
        }

        public override void Read(PacketBuffer buffer) {
            this.EntityName = buffer.ReadString();
this.Action = buffer.ReadByte();
this.ObjectiveName = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString(this.EntityName);
buffer.WriteByte(this.Action);
buffer.WriteString(this.ObjectiveName);
        }
    }
}
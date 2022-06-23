namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class UpdateCommandBlockMinecartPacket : Packet {

        public int EntityID { get; private set; }
public string? Command { get; private set; }
public bool TrackOutput { get; private set; }

        public UpdateCommandBlockMinecartPacket() { }

        public UpdateCommandBlockMinecartPacket(int entityid, string command, bool trackoutput) {
            this.EntityID = entityid;
this.Command = command;
this.TrackOutput = trackoutput;
        }

        public override void Read(PacketBuffer buffer) {
            this.EntityID = buffer.ReadVarInt();
this.Command = buffer.ReadString();
this.TrackOutput = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.EntityID);
buffer.WriteString(this.Command!);
buffer.WriteBoolean(this.TrackOutput);
        }
    }
}
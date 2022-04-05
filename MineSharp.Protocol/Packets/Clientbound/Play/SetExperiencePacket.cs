namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class SetExperiencePacket : Packet {

        public float Experiencebar { get; private set; }
public int Level { get; private set; }
public int TotalExperience { get; private set; }

        public SetExperiencePacket() { }

        public SetExperiencePacket(float experiencebar, int level, int totalexperience) {
            this.Experiencebar = experiencebar;
this.Level = level;
this.TotalExperience = totalexperience;
        }

        public override void Read(PacketBuffer buffer) {
            this.Experiencebar = buffer.ReadFloat();
this.Level = buffer.ReadVarInt();
this.TotalExperience = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteFloat(this.Experiencebar);
buffer.WriteVarInt(this.Level);
buffer.WriteVarInt(this.TotalExperience);
        }
    }
}
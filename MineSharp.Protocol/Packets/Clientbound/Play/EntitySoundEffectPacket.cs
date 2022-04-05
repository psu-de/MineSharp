namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class EntitySoundEffectPacket : Packet {

        public int SoundID { get; private set; }
public int /* TODO: Enum! */ SoundCategory { get; private set; }
public int EntityID { get; private set; }
public float Volume { get; private set; }
public float Pitch { get; private set; }

        public EntitySoundEffectPacket() { }

        public EntitySoundEffectPacket(int soundid, int /* TODO: Enum! */ soundcategory, int entityid, float volume, float pitch) {
            this.SoundID = soundid;
this.SoundCategory = soundcategory;
this.EntityID = entityid;
this.Volume = volume;
this.Pitch = pitch;
        }

        public override void Read(PacketBuffer buffer) {
            this.SoundID = buffer.ReadVarInt();
this.SoundCategory = buffer.ReadVarInt();
this.EntityID = buffer.ReadVarInt();
this.Volume = buffer.ReadFloat();
this.Pitch = buffer.ReadFloat();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.SoundID);
buffer.WriteVarInt(this.SoundCategory);
buffer.WriteVarInt(this.EntityID);
buffer.WriteFloat(this.Volume);
buffer.WriteFloat(this.Pitch);
        }
    }
}
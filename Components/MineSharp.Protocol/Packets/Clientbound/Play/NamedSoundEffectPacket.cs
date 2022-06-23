using MineSharp.Core.Types;
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class NamedSoundEffectPacket : Packet {

        public Identifier? SoundName { get; private set; }
        public int /* TODO: Enum! */ SoundCategory { get; private set; }
        public int EffectPositionX { get; private set; }
        public int EffectPositionY { get; private set; }
        public int EffectPositionZ { get; private set; }
        public float Volume { get; private set; }
        public float Pitch { get; private set; }

        public NamedSoundEffectPacket() { }

        public NamedSoundEffectPacket(Identifier? soundname, int /* TODO: Enum! */ soundcategory, int effectpositionx, int effectpositiony, int effectpositionz, float volume, float pitch) {
            this.SoundName = soundname;
            this.SoundCategory = soundcategory;
            this.EffectPositionX = effectpositionx;
            this.EffectPositionY = effectpositiony;
            this.EffectPositionZ = effectpositionz;
            this.Volume = volume;
            this.Pitch = pitch;
        }

        public override void Read(PacketBuffer buffer) {
            this.SoundName = buffer.ReadIdentifier();
            this.SoundCategory = buffer.ReadVarInt();
            this.EffectPositionX = buffer.ReadInt();
            this.EffectPositionY = buffer.ReadInt();
            this.EffectPositionZ = buffer.ReadInt();
            this.Volume = buffer.ReadFloat();
            this.Pitch = buffer.ReadFloat();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteIdentifier(this.SoundName!);
            buffer.WriteVarInt(this.SoundCategory);
            buffer.WriteInt(this.EffectPositionX);
            buffer.WriteInt(this.EffectPositionY);
            buffer.WriteInt(this.EffectPositionZ);
            buffer.WriteFloat(this.Volume);
            buffer.WriteFloat(this.Pitch);
        }
    }
}
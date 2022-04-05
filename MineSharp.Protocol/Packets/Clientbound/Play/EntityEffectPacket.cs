namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class EntityEffectPacket : Packet {

        public int EntityID { get; private set; }
public byte EffectID { get; private set; }
public byte Amplifier { get; private set; }
public int Duration { get; private set; }
public byte Flags { get; private set; }

        public EntityEffectPacket() { }

        public EntityEffectPacket(int entityid, byte effectid, byte amplifier, int duration, byte flags) {
            this.EntityID = entityid;
this.EffectID = effectid;
this.Amplifier = amplifier;
this.Duration = duration;
this.Flags = flags;
        }

        public override void Read(PacketBuffer buffer) {
            this.EntityID = buffer.ReadVarInt();
this.EffectID = buffer.ReadByte();
this.Amplifier = buffer.ReadByte();
this.Duration = buffer.ReadVarInt();
this.Flags = buffer.ReadByte();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.EntityID);
buffer.WriteByte(this.EffectID);
buffer.WriteByte(this.Amplifier);
buffer.WriteVarInt(this.Duration);
buffer.WriteByte(this.Flags);
        }
    }
}
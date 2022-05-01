using MineSharp.Data.Effects;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class RemoveEntityEffectPacket : Packet {

        public int EntityID { get; private set; }
        public EffectType EffectID { get; private set; }

        public RemoveEntityEffectPacket() { }

        public RemoveEntityEffectPacket(int entityid, EffectType effectid) {
            this.EntityID = entityid;
            this.EffectID = effectid;
        }

        public override void Read(PacketBuffer buffer) {
            this.EntityID = buffer.ReadVarInt();
            this.EffectID = (EffectType)buffer.ReadByte();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.EntityID);
            buffer.WriteByte((byte)this.EffectID);
        }
    }
}
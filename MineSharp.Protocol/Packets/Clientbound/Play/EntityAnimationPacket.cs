namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class EntityAnimationPacket : Packet {

        public int EntityID { get; private set; }
        public byte Animation { get; private set; }

        public EntityAnimationPacket() { }

        public EntityAnimationPacket(int entityid, byte animation) {
            this.EntityID = entityid;
            this.Animation = animation;
        }

        public override void Read(PacketBuffer buffer) {
            this.EntityID = buffer.ReadVarInt();
            this.Animation = buffer.ReadByte();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.EntityID);
            buffer.WriteByte(this.Animation);
        }
    }
}
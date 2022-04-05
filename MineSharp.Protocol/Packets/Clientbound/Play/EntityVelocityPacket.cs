namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class EntityVelocityPacket : Packet {

        public int EntityID { get; private set; }
public short VelocityX { get; private set; }
public short VelocityY { get; private set; }
public short VelocityZ { get; private set; }

        public EntityVelocityPacket() { }

        public EntityVelocityPacket(int entityid, short velocityx, short velocityy, short velocityz) {
            this.EntityID = entityid;
this.VelocityX = velocityx;
this.VelocityY = velocityy;
this.VelocityZ = velocityz;
        }

        public override void Read(PacketBuffer buffer) {
            this.EntityID = buffer.ReadVarInt();
this.VelocityX = buffer.ReadShort();
this.VelocityY = buffer.ReadShort();
this.VelocityZ = buffer.ReadShort();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.EntityID);
buffer.WriteShort(this.VelocityX);
buffer.WriteShort(this.VelocityY);
buffer.WriteShort(this.VelocityZ);
        }
    }
}
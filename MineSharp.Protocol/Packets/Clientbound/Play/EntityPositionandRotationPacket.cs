using MineSharp.Core.Types;
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class EntityPositionAndRotationPacket : Packet {

        public int EntityID { get; private set; }
        public short DeltaX { get; private set; }
        public short DeltaY { get; private set; }
        public short DeltaZ { get; private set; }
        public Angle? Yaw { get; private set; }
        public Angle? Pitch { get; private set; }
        public bool OnGround { get; private set; }

        public EntityPositionAndRotationPacket() { }

        public EntityPositionAndRotationPacket(int entityid, short deltax, short deltay, short deltaz, Angle? yaw, Angle? pitch, bool onground) {
            this.EntityID = entityid;
            this.DeltaX = deltax;
            this.DeltaY = deltay;
            this.DeltaZ = deltaz;
            this.Yaw = yaw;
            this.Pitch = pitch;
            this.OnGround = onground;
        }

        public override void Read(PacketBuffer buffer) {
            this.EntityID = buffer.ReadVarInt();
            this.DeltaX = buffer.ReadShort();
            this.DeltaY = buffer.ReadShort();
            this.DeltaZ = buffer.ReadShort();
            this.Yaw = buffer.ReadAngle();
            this.Pitch = buffer.ReadAngle();
            this.OnGround = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.EntityID);
            buffer.WriteShort(this.DeltaX);
            buffer.WriteShort(this.DeltaY);
            buffer.WriteShort(this.DeltaZ);
            buffer.WriteAngle(this.Yaw);
            buffer.WriteAngle(this.Pitch);
            buffer.WriteBoolean(this.OnGround);
        }
    }
}
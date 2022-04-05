using MineSharp.Core.Types;
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class EntityTeleportPacket : Packet {

        public int EntityID { get; private set; }
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        public Angle? Yaw { get; private set; }
        public Angle? Pitch { get; private set; }
        public bool OnGround { get; private set; }

        public EntityTeleportPacket() { }

        public EntityTeleportPacket(int entityid, double x, double y, double z, Angle? yaw, Angle? pitch, bool onground) {
            this.EntityID = entityid;
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Yaw = yaw;
            this.Pitch = pitch;
            this.OnGround = onground;
        }

        public override void Read(PacketBuffer buffer) {
            this.EntityID = buffer.ReadVarInt();
            this.X = buffer.ReadDouble();
            this.Y = buffer.ReadDouble();
            this.Z = buffer.ReadDouble();
            this.Yaw = buffer.ReadAngle();
            this.Pitch = buffer.ReadAngle();
            this.OnGround = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.EntityID);
            buffer.WriteDouble(this.X);
            buffer.WriteDouble(this.Y);
            buffer.WriteDouble(this.Z);
            buffer.WriteAngle(this.Yaw);
            buffer.WriteAngle(this.Pitch);
            buffer.WriteBoolean(this.OnGround);
        }
    }
}
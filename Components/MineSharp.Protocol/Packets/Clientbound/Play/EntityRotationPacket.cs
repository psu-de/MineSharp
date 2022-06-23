using MineSharp.Core.Types;
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class EntityRotationPacket : Packet {

        public int EntityID { get; private set; }
        public Angle? Yaw { get; private set; }
        public Angle? Pitch { get; private set; }
        public bool OnGround { get; private set; }

        public EntityRotationPacket() { }

        public EntityRotationPacket(int entityid, Angle yaw, Angle pitch, bool onground) {
            this.EntityID = entityid;
            this.Yaw = yaw;
            this.Pitch = pitch;
            this.OnGround = onground;
        }

        public override void Read(PacketBuffer buffer) {
            this.EntityID = buffer.ReadVarInt();
            this.Yaw = buffer.ReadAngle();
            this.Pitch = buffer.ReadAngle();
            this.OnGround = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.EntityID);
            buffer.WriteAngle(this.Yaw!);
            buffer.WriteAngle(this.Pitch!);
            buffer.WriteBoolean(this.OnGround);
        }
    }
}
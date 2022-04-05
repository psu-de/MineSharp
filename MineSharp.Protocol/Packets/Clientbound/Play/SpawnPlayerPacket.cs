using MineSharp.Core.Types;
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class SpawnPlayerPacket : Packet {

        public int EntityID { get; private set; }
        public UUID PlayerUUID { get; private set; }
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        public Angle? Yaw { get; private set; }
        public Angle? Pitch { get; private set; }

        public SpawnPlayerPacket() { }

        public SpawnPlayerPacket(int entityid, UUID playeruuid, double x, double y, double z, Angle? yaw, Angle? pitch) {
            this.EntityID = entityid;
            this.PlayerUUID = playeruuid;
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Yaw = yaw;
            this.Pitch = pitch;
        }

        public override void Read(PacketBuffer buffer) {
            this.EntityID = buffer.ReadVarInt();
            this.PlayerUUID = buffer.ReadUUID();
            this.X = buffer.ReadDouble();
            this.Y = buffer.ReadDouble();
            this.Z = buffer.ReadDouble();
            this.Yaw = buffer.ReadAngle();
            this.Pitch = buffer.ReadAngle();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.EntityID);
            buffer.WriteUUID(this.PlayerUUID);
            buffer.WriteDouble(this.X);
            buffer.WriteDouble(this.Y);
            buffer.WriteDouble(this.Z);
            buffer.WriteAngle(this.Yaw);
            buffer.WriteAngle(this.Pitch);
        }
    }
}
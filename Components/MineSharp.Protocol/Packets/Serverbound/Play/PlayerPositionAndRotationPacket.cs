namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class PlayerPositionAndRotationPacket : Packet {

        public double X { get; private set; }
        public double FeetY { get; private set; }
        public double Z { get; private set; }
        public float Yaw { get; private set; }
        public float Pitch { get; private set; }
        public bool OnGround { get; private set; }

        public PlayerPositionAndRotationPacket() { }

        public PlayerPositionAndRotationPacket(double x, double feety, double z, float yaw, float pitch, bool onground) {
            this.X = x;
            this.FeetY = feety;
            this.Z = z;
            this.Yaw = yaw;
            this.Pitch = pitch;
            this.OnGround = onground;
        }

        public override void Read(PacketBuffer buffer) {
            this.X = buffer.ReadDouble();
            this.FeetY = buffer.ReadDouble();
            this.Z = buffer.ReadDouble();
            this.Yaw = buffer.ReadFloat();
            this.Pitch = buffer.ReadFloat();
            this.OnGround = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteDouble(this.X);
            buffer.WriteDouble(this.FeetY);
            buffer.WriteDouble(this.Z);
            buffer.WriteFloat(this.Yaw);
            buffer.WriteFloat(this.Pitch);
            buffer.WriteBoolean(this.OnGround);
        }
    }
}
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class VehicleMovePacket : Packet {

        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        public float Yaw { get; private set; }
        public float Pitch { get; private set; }

        public VehicleMovePacket() { }

        public VehicleMovePacket(double x, double y, double z, float yaw, float pitch) {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Yaw = yaw;
            this.Pitch = pitch;
        }

        public override void Read(PacketBuffer buffer) {
            this.X = buffer.ReadDouble();
            this.Y = buffer.ReadDouble();
            this.Z = buffer.ReadDouble();
            this.Yaw = buffer.ReadFloat();
            this.Pitch = buffer.ReadFloat();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteDouble(this.X);
            buffer.WriteDouble(this.Y);
            buffer.WriteDouble(this.Z);
            buffer.WriteFloat(this.Yaw);
            buffer.WriteFloat(this.Pitch);
        }
    }
}
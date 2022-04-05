namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class WorldBorderCenterPacket : Packet {

        public double X { get; private set; }
public double Z { get; private set; }

        public WorldBorderCenterPacket() { }

        public WorldBorderCenterPacket(double x, double z) {
            this.X = x;
this.Z = z;
        }

        public override void Read(PacketBuffer buffer) {
            this.X = buffer.ReadDouble();
this.Z = buffer.ReadDouble();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteDouble(this.X);
buffer.WriteDouble(this.Z);
        }
    }
}
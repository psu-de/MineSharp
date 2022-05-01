namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class PlayerPositionPacket : Packet {

        public double X { get; private set; }
public double FeetY { get; private set; }
public double Z { get; private set; }
public bool OnGround { get; private set; }

        public PlayerPositionPacket() { }

        public PlayerPositionPacket(double x, double feety, double z, bool onground) {
            this.X = x;
this.FeetY = feety;
this.Z = z;
this.OnGround = onground;
        }

        public override void Read(PacketBuffer buffer) {
            this.X = buffer.ReadDouble();
this.FeetY = buffer.ReadDouble();
this.Z = buffer.ReadDouble();
this.OnGround = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteDouble(this.X);
buffer.WriteDouble(this.FeetY);
buffer.WriteDouble(this.Z);
buffer.WriteBoolean(this.OnGround);
        }
    }
}
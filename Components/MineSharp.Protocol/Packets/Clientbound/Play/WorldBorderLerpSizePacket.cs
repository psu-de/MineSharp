namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class WorldBorderLerpSizePacket : Packet {

        public double OldDiameter { get; private set; }
public double NewDiameter { get; private set; }
public long Speed { get; private set; }

        public WorldBorderLerpSizePacket() { }

        public WorldBorderLerpSizePacket(double olddiameter, double newdiameter, long speed) {
            this.OldDiameter = olddiameter;
this.NewDiameter = newdiameter;
this.Speed = speed;
        }

        public override void Read(PacketBuffer buffer) {
            this.OldDiameter = buffer.ReadDouble();
this.NewDiameter = buffer.ReadDouble();
this.Speed = buffer.ReadVarLong();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteDouble(this.OldDiameter);
buffer.WriteDouble(this.NewDiameter);
buffer.WriteVarLong(this.Speed);
        }
    }
}
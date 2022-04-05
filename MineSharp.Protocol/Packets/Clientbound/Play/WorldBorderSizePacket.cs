namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class WorldBorderSizePacket : Packet {

        public double Diameter { get; private set; }

        public WorldBorderSizePacket() { }

        public WorldBorderSizePacket(double diameter) {
            this.Diameter = diameter;
        }

        public override void Read(PacketBuffer buffer) {
            this.Diameter = buffer.ReadDouble();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteDouble(this.Diameter);
        }
    }
}
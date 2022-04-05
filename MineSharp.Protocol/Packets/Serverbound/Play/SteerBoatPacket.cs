namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class SteerBoatPacket : Packet {

        public bool Leftpaddleturning { get; private set; }
public bool Rightpaddleturning { get; private set; }

        public SteerBoatPacket() { }

        public SteerBoatPacket(bool leftpaddleturning, bool rightpaddleturning) {
            this.Leftpaddleturning = leftpaddleturning;
this.Rightpaddleturning = rightpaddleturning;
        }

        public override void Read(PacketBuffer buffer) {
            this.Leftpaddleturning = buffer.ReadBoolean();
this.Rightpaddleturning = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteBoolean(this.Leftpaddleturning);
buffer.WriteBoolean(this.Rightpaddleturning);
        }
    }
}
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class TimeUpdatePacket : Packet {

        public long WorldAge { get; private set; }
public long Timeofday { get; private set; }

        public TimeUpdatePacket() { }

        public TimeUpdatePacket(long worldage, long timeofday) {
            this.WorldAge = worldage;
this.Timeofday = timeofday;
        }

        public override void Read(PacketBuffer buffer) {
            this.WorldAge = buffer.ReadLong();
this.Timeofday = buffer.ReadLong();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteLong(this.WorldAge);
buffer.WriteLong(this.Timeofday);
        }
    }
}
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class StatisticsPacket : Packet {

        public int Count { get; private set; }
public int StatisticID { get; private set; }
public int Value { get; private set; }

        public StatisticsPacket() { }

        public StatisticsPacket(int count, int statisticid, int value) {
            this.Count = count;
this.StatisticID = statisticid;
this.Value = value;
        }

        public override void Read(PacketBuffer buffer) {
            this.Count = buffer.ReadVarInt();
this.StatisticID = buffer.ReadVarInt();
this.Value = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.Count);
buffer.WriteVarInt(this.StatisticID);
buffer.WriteVarInt(this.Value);
        }
    }
}
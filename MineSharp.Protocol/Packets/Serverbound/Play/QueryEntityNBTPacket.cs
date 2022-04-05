namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class QueryEntityNBTPacket : Packet {

        public int TransactionID { get; private set; }
public int EntityID { get; private set; }

        public QueryEntityNBTPacket() { }

        public QueryEntityNBTPacket(int transactionid, int entityid) {
            this.TransactionID = transactionid;
this.EntityID = entityid;
        }

        public override void Read(PacketBuffer buffer) {
            this.TransactionID = buffer.ReadVarInt();
this.EntityID = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.TransactionID);
buffer.WriteVarInt(this.EntityID);
        }
    }
}
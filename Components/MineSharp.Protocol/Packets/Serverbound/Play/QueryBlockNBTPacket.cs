using MineSharp.Core.Types;

namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class QueryBlockNBTPacket : Packet {

        public int TransactionID { get; private set; }
        public Position? Location { get; private set; }

        public QueryBlockNBTPacket() { }

        public QueryBlockNBTPacket(int transactionid, Position location) {
            this.TransactionID = transactionid;
            this.Location = location;
        }

        public override void Read(PacketBuffer buffer) {
            this.TransactionID = buffer.ReadVarInt();
            this.Location = buffer.ReadPosition();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.TransactionID);
            buffer.WritePosition(this.Location!);
        }
    }
}
using MineSharp.Core.Types;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class BlockChangePacket : Packet {

        public Position? Location { get; private set; }
        public int BlockID { get; private set; }

        public BlockChangePacket() { }

        public BlockChangePacket(Position location, int blockid) {
            this.Location = location;
            this.BlockID = blockid;
        }

        public override void Read(PacketBuffer buffer) {
            this.Location = buffer.ReadPosition();
            this.BlockID = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WritePosition(this.Location!);
            buffer.WriteVarInt(this.BlockID);
        }
    }
}
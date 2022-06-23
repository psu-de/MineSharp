using MineSharp.Core.Types;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class BlockActionPacket : Packet {

        public Position? Location { get; private set; }
        public byte ActionID { get; private set; }
        public byte ActionParam { get; private set; }
        public int BlockType { get; private set; }

        public BlockActionPacket() { }

        public BlockActionPacket(Position location, byte actionid, byte actionparam, int blocktype) {
            this.Location = location;
            this.ActionID = actionid;
            this.ActionParam = actionparam;
            this.BlockType = blocktype;
        }

        public override void Read(PacketBuffer buffer) {
            this.Location = buffer.ReadPosition();
            this.ActionID = buffer.ReadByte();
            this.ActionParam = buffer.ReadByte();
            this.BlockType = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WritePosition(this.Location!);
            buffer.WriteByte(this.ActionID);
            buffer.WriteByte(this.ActionParam);
            buffer.WriteVarInt(this.BlockType);
        }
    }
}
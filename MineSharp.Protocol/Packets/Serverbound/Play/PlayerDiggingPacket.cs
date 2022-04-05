using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;

namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class PlayerDiggingPacket : Packet {

        public DiggingStatus Status { get; set; }
        public Position? Location { get; set; }
        public BlockFace Face { get; set; }

        public PlayerDiggingPacket() { }

        public PlayerDiggingPacket(DiggingStatus status, Position? location, BlockFace face) {
            this.Status = status;
            this.Location = location;
            this.Face = face;
        }

        public override void Read(PacketBuffer buffer) {
            this.Status = (DiggingStatus)buffer.ReadVarInt();
            this.Location = buffer.ReadPosition();
            this.Face = (BlockFace)buffer.ReadByte();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt((int)this.Status);
            buffer.WritePosition(this.Location);
            buffer.WriteByte((byte)this.Face);
        }
    }
}
using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class AcknowledgePlayerDiggingPacket : Packet {

        public Position? Location { get; private set; }
        public int Block { get; private set; }
        public DiggingStatus Status { get; private set; }
        public bool Successful { get; private set; }

        public AcknowledgePlayerDiggingPacket() { }

        public AcknowledgePlayerDiggingPacket(Position? location, int block, DiggingStatus status, bool successful) {
            this.Location = location;
            this.Block = block;
            this.Status = status;
            this.Successful = successful;
        }

        public override void Read(PacketBuffer buffer) {
            this.Location = buffer.ReadPosition();
            this.Block = buffer.ReadVarInt();
            this.Status = (DiggingStatus)buffer.ReadVarInt();
            this.Successful = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WritePosition(this.Location);
            buffer.WriteVarInt(this.Block);
            buffer.WriteVarInt((int)this.Status);
            buffer.WriteBoolean(this.Successful);
        }
    }
}
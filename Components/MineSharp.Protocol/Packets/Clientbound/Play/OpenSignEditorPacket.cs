using MineSharp.Core.Types;
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class OpenSignEditorPacket : Packet {

        public Position? Location { get; private set; }

        public OpenSignEditorPacket() { }

        public OpenSignEditorPacket(Position? location) {
            this.Location = location;
        }

        public override void Read(PacketBuffer buffer) {
            this.Location = buffer.ReadPosition();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WritePosition(this.Location);
        }
    }
}
using MineSharp.Core.Types;
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class SpawnPositionPacket : Packet {

        public Position? Location { get; private set; }
        public float Angle { get; private set; }

        public SpawnPositionPacket() { }

        public SpawnPositionPacket(Position? location, float angle) {
            this.Location = location;
            this.Angle = angle;
        }

        public override void Read(PacketBuffer buffer) {
            this.Location = buffer.ReadPosition();
            this.Angle = buffer.ReadFloat();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WritePosition(this.Location!);
            buffer.WriteFloat(this.Angle);
        }
    }
}
using MineSharp.Core.Types;

namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class SpectatePacket : Packet {

        public UUID? TargetPlayer { get; private set; }

        public SpectatePacket() { }

        public SpectatePacket(UUID? targetplayer) {
            this.TargetPlayer = targetplayer;
        }

        public override void Read(PacketBuffer buffer) {
            this.TargetPlayer = buffer.ReadUUID();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteUUID((UUID)this.TargetPlayer);
        }
    }
}
using MineSharp.Core.Types.Enums;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class ChangeGameStatePacket : Packet {

        public GameStateReason Reason { get; private set; }
        public float Value { get; private set; }

        public ChangeGameStatePacket() { }

        public ChangeGameStatePacket(GameStateReason reason, float value) {
            this.Reason = reason;
            this.Value = value;
        }

        public override void Read(PacketBuffer buffer) {
            this.Reason = (GameStateReason)buffer.ReadByte();
            this.Value = buffer.ReadFloat();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteByte((byte)this.Reason);
            buffer.WriteFloat(this.Value);
        }
    }
}
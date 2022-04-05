namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class ChangeGameStatePacket : Packet {

        public byte Reason { get; private set; }
public float Value { get; private set; }

        public ChangeGameStatePacket() { }

        public ChangeGameStatePacket(byte reason, float value) {
            this.Reason = reason;
this.Value = value;
        }

        public override void Read(PacketBuffer buffer) {
            this.Reason = buffer.ReadByte();
this.Value = buffer.ReadFloat();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteByte(this.Reason);
buffer.WriteFloat(this.Value);
        }
    }
}
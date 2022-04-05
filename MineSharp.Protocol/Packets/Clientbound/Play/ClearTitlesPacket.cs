namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class ClearTitlesPacket : Packet {

        public bool Reset { get; private set; }

        public ClearTitlesPacket() { }

        public ClearTitlesPacket(bool reset) {
            this.Reset = reset;
        }

        public override void Read(PacketBuffer buffer) {
            this.Reset = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteBoolean(this.Reset);
        }
    }
}
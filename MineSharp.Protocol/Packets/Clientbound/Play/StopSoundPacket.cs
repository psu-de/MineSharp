namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class StopSoundPacket : Packet {

        public byte Flags { get; private set; }

        public StopSoundPacket() { }

        public StopSoundPacket(byte flags) {
            this.Flags = flags;
        }

        public override void Read(PacketBuffer buffer) {
            this.Flags = buffer.ReadByte();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteByte(this.Flags);
        }
    }
}
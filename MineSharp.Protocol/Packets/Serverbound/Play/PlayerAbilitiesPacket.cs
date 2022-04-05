namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class PlayerAbilitiesPacket : Packet {

        public byte Flags { get; private set; }

        public PlayerAbilitiesPacket() { }

        public PlayerAbilitiesPacket(byte flags) {
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
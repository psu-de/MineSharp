namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class SetDifficultyPacket : Packet {

        public byte Newdifficulty { get; private set; }

        public SetDifficultyPacket() { }

        public SetDifficultyPacket(byte newdifficulty) {
            this.Newdifficulty = newdifficulty;
        }

        public override void Read(PacketBuffer buffer) {
            this.Newdifficulty = buffer.ReadByte();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteByte(this.Newdifficulty);
        }
    }
}
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class ServerDifficultyPacket : Packet {

        public byte Difficulty { get; private set; }
        public bool Difficultylocked { get; private set; }

        public ServerDifficultyPacket() { }

        public ServerDifficultyPacket(byte difficulty, bool difficultylocked) {
            this.Difficulty = difficulty;
            this.Difficultylocked = difficultylocked;
        }

        public override void Read(PacketBuffer buffer) {
            this.Difficulty = buffer.ReadByte();
            this.Difficultylocked = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteByte(this.Difficulty);
            buffer.WriteBoolean(this.Difficultylocked);
        }
    }
}
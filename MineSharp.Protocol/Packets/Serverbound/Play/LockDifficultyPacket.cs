namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class LockDifficultyPacket : Packet {

        public bool Locked { get; private set; }

        public LockDifficultyPacket() { }

        public LockDifficultyPacket(bool locked) {
            this.Locked = locked;
        }

        public override void Read(PacketBuffer buffer) {
            this.Locked = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteBoolean(this.Locked);
        }
    }
}
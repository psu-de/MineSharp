namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class PlayerMovementPacket : Packet {

        public bool OnGround { get; private set; }

        public PlayerMovementPacket() { }

        public PlayerMovementPacket(bool onground) {
            this.OnGround = onground;
        }

        public override void Read(PacketBuffer buffer) {
            this.OnGround = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteBoolean(this.OnGround);
        }
    }
}
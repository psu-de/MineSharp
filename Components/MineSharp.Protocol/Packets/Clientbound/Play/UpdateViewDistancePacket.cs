namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class UpdateViewDistancePacket : Packet {

        public int ViewDistance { get; private set; }

        public UpdateViewDistancePacket() { }

        public UpdateViewDistancePacket(int viewdistance) {
            this.ViewDistance = viewdistance;
        }

        public override void Read(PacketBuffer buffer) {
            this.ViewDistance = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.ViewDistance);
        }
    }
}
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class CameraPacket : Packet {

        public int CameraID { get; private set; }

        public CameraPacket() { }

        public CameraPacket(int cameraid) {
            this.CameraID = cameraid;
        }

        public override void Read(PacketBuffer buffer) {
            this.CameraID = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.CameraID);
        }
    }
}
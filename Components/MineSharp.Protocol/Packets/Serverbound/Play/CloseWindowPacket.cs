namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class CloseWindowPacket : Packet {

        public byte WindowID { get; private set; }

        public CloseWindowPacket() { }

        public CloseWindowPacket(byte windowid) {
            this.WindowID = windowid;
        }

        public override void Read(PacketBuffer buffer) {
            this.WindowID = buffer.ReadByte();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteByte(this.WindowID);
        }
    }
}
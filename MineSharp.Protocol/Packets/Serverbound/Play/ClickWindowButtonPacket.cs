namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class ClickWindowButtonPacket : Packet {

        public byte WindowID { get; private set; }
public byte ButtonID { get; private set; }

        public ClickWindowButtonPacket() { }

        public ClickWindowButtonPacket(byte windowid, byte buttonid) {
            this.WindowID = windowid;
this.ButtonID = buttonid;
        }

        public override void Read(PacketBuffer buffer) {
            this.WindowID = buffer.ReadByte();
this.ButtonID = buffer.ReadByte();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteByte(this.WindowID);
buffer.WriteByte(this.ButtonID);
        }
    }
}
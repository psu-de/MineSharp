namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class WindowPropertyPacket : Packet {

        public byte WindowID { get; private set; }
public short Property { get; private set; }
public short Value { get; private set; }

        public WindowPropertyPacket() { }

        public WindowPropertyPacket(byte windowid, short property, short value) {
            this.WindowID = windowid;
this.Property = property;
this.Value = value;
        }

        public override void Read(PacketBuffer buffer) {
            this.WindowID = buffer.ReadByte();
this.Property = buffer.ReadShort();
this.Value = buffer.ReadShort();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteByte(this.WindowID);
buffer.WriteShort(this.Property);
buffer.WriteShort(this.Value);
        }
    }
}
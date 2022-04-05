using MineSharp.Core.Types;
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class OpenWindowPacket : Packet {

        public int WindowID { get; private set; }
        public int WindowType { get; private set; }
        public Chat? WindowTitle { get; private set; }

        public OpenWindowPacket() { }

        public OpenWindowPacket(int windowid, int windowtype, Chat? windowtitle) {
            this.WindowID = windowid;
            this.WindowType = windowtype;
            this.WindowTitle = windowtitle;
        }

        public override void Read(PacketBuffer buffer) {
            this.WindowID = buffer.ReadVarInt();
            this.WindowType = buffer.ReadVarInt();
            this.WindowTitle = buffer.ReadChat();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.WindowID);
            buffer.WriteVarInt(this.WindowType);
            buffer.WriteChat(this.WindowTitle);
        }
    }
}
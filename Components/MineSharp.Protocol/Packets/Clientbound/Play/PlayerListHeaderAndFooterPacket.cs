using MineSharp.Core.Types;
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class PlayerListHeaderAndFooterPacket : Packet {

        public Chat? Header { get; private set; }
        public Chat? Footer { get; private set; }

        public PlayerListHeaderAndFooterPacket() { }

        public PlayerListHeaderAndFooterPacket(Chat header, Chat footer) {
            this.Header = header;
            this.Footer = footer;
        }

        public override void Read(PacketBuffer buffer) {
            this.Header = buffer.ReadChat();
            this.Footer = buffer.ReadChat();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteChat(this.Header!);
            buffer.WriteChat(this.Footer!);
        }
    }
}
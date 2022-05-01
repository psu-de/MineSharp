using MineSharp.Core.Types;
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class SetTitleSubTitlePacket : Packet {

        public Chat? SubtitleText { get; private set; }

        public SetTitleSubTitlePacket() { }

        public SetTitleSubTitlePacket(Chat? subtitletext) {
            this.SubtitleText = subtitletext;
        }

        public override void Read(PacketBuffer buffer) {
            this.SubtitleText = buffer.ReadChat();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteChat(this.SubtitleText);
        }
    }
}
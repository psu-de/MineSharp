using MineSharp.Core.Types;
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class SetTitleTextPacket : Packet {

        public Chat? TitleText { get; private set; }

        public SetTitleTextPacket() { }

        public SetTitleTextPacket(Chat? titletext) {
            this.TitleText = titletext;
        }

        public override void Read(PacketBuffer buffer) {
            this.TitleText = buffer.ReadChat();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteChat(this.TitleText!);
        }
    }
}
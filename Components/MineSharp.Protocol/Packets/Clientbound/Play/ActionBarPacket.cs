using MineSharp.Core.Types;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class ActionBarPacket : Packet {

        public Chat? Actionbartext { get; private set; }

        public ActionBarPacket() { }

        public ActionBarPacket(Chat? actionbartext) {
            this.Actionbartext = actionbartext;
        }

        public override void Read(PacketBuffer buffer) {
            this.Actionbartext = buffer.ReadChat();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteChat(this.Actionbartext);
        }
    }
}
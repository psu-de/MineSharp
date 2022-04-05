using MineSharp.Core.Types;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class ChatMessagePacket : Packet {

        public Chat? JSONData { get; private set; }
        public byte Position { get; private set; }
        public UUID Sender { get; private set; }

        public ChatMessagePacket() { }

        public ChatMessagePacket(Chat? jsondata, byte position, UUID sender) {
            this.JSONData = jsondata;
            this.Position = position;
            this.Sender = sender;
        }

        public override void Read(PacketBuffer buffer) {
            this.JSONData = buffer.ReadChat();
            this.Position = buffer.ReadByte();
            this.Sender = buffer.ReadUUID();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteChat(this.JSONData);
            buffer.WriteByte(this.Position);
            buffer.WriteUUID(this.Sender);
        }
    }
}
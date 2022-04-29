using MineSharp.Core.Types;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class PluginMessagePacket : Packet {

        public Identifier? Channel { get; private set; }
        public byte[]? Data { get; private set; }

        public PluginMessagePacket() { }

        public PluginMessagePacket(Identifier? channel, byte[]? data) {
            this.Channel = channel;
            this.Data = data;
        }

        public override void Read(PacketBuffer buffer) {
            this.Channel = buffer.ReadIdentifier();
            this.Data = buffer.ReadByteArray();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteIdentifier(this.Channel);
            buffer.WriteByteArray(this.Data);
        }
    }
}
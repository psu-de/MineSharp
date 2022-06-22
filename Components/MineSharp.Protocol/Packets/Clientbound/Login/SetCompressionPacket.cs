namespace MineSharp.Protocol.Packets.Clientbound.Login {
    public class SetCompressionPacket : Packet {

        public int Threshold { get; private set; }

        public SetCompressionPacket() { }
        public SetCompressionPacket(int threshold) {
            this.Threshold = threshold;
        }

        public override async Task Handle(MinecraftClient client) {
            Logger.Debug("Enabling compression, threshold=" + this.Threshold);
            client.SetCompressionThreshold(this.Threshold);
            await base.Handle(client);
        }

        public override void Read(PacketBuffer buffer) {
            this.Threshold = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.Threshold);
        }
    }
}

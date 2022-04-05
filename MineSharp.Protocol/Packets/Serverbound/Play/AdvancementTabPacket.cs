namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class AdvancementTabPacket : Packet {

        public int /* TODO: Enum! */ Action { get; private set; }

        public AdvancementTabPacket() { }

        public AdvancementTabPacket(int /* TODO: Enum! */ action) {
            this.Action = action;
        }

        public override void Read(PacketBuffer buffer) {
            this.Action = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.Action);
        }
    }
}
namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class NameItemPacket : Packet {

        public string? Itemname { get; private set; }

        public NameItemPacket() { }

        public NameItemPacket(string itemname) {
            this.Itemname = itemname;
        }

        public override void Read(PacketBuffer buffer) {
            this.Itemname = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString(this.Itemname!);
        }
    }
}
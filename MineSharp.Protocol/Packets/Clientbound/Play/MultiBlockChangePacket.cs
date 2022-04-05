namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class MultiBlockChangePacket : Packet {

        public long Chunksectionposition { get; private set; }
        public bool Inverse { get; private set; }
        public long[] Blocks { get; private set; }

        public MultiBlockChangePacket() { }

        public MultiBlockChangePacket(long chunksectionposition, bool inverse, long[] blocks) {
            this.Chunksectionposition = chunksectionposition;
            this.Inverse = inverse;
            this.Blocks = blocks;
        }

        public override void Read(PacketBuffer buffer) {
            this.Chunksectionposition = buffer.ReadLong();
            this.Inverse = buffer.ReadBoolean();
            this.Blocks = buffer.ReadVarLongArray();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteLong(this.Chunksectionposition);
            buffer.WriteBoolean(this.Inverse);
            buffer.WriteVarLongArray(this.Blocks);
        }
    }
}
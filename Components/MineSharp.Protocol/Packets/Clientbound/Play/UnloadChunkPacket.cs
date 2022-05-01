namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class UnloadChunkPacket : Packet {

        public int ChunkX { get; private set; }
public int ChunkZ { get; private set; }

        public UnloadChunkPacket() { }

        public UnloadChunkPacket(int chunkx, int chunkz) {
            this.ChunkX = chunkx;
this.ChunkZ = chunkz;
        }

        public override void Read(PacketBuffer buffer) {
            this.ChunkX = buffer.ReadInt();
this.ChunkZ = buffer.ReadInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteInt(this.ChunkX);
buffer.WriteInt(this.ChunkZ);
        }
    }
}
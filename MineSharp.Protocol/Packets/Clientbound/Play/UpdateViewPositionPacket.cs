namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class UpdateViewPositionPacket : Packet {

        public int ChunkX { get; private set; }
public int ChunkZ { get; private set; }

        public UpdateViewPositionPacket() { }

        public UpdateViewPositionPacket(int chunkx, int chunkz) {
            this.ChunkX = chunkx;
this.ChunkZ = chunkz;
        }

        public override void Read(PacketBuffer buffer) {
            this.ChunkX = buffer.ReadVarInt();
this.ChunkZ = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.ChunkX);
buffer.WriteVarInt(this.ChunkZ);
        }
    }
}
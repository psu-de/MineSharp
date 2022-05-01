using fNbt;
using MineSharp.Core.Types;

namespace MineSharp.Protocol.Packets.Clientbound.Play {

    public class UpdateLightPacket : Packet {

        public int ChunkX { get; private set; }
        public int ChunkZ { get; private set; }
        public bool TrustEdges { get; private set; }
        public BitSet SkyLightMask { get; private set; }
        public BitSet BlockLightMask { get; private set; }
        public BitSet EmptySkyLightMask { get; private set; }
        public BitSet EmptyBlockLightMask { get; private set; }
        public byte[][] SkyLightArray { get; private set; }
        public byte[][] BlockLightArray { get; private set; }

        public override void Read(PacketBuffer buffer) {
            this.ChunkX = buffer.ReadVarInt();
            this.ChunkZ = buffer.ReadVarInt();
            this.TrustEdges = buffer.ReadBoolean();
            this.SkyLightMask = buffer.ReadBitSet();
            this.BlockLightMask = buffer.ReadBitSet();
            this.EmptySkyLightMask = buffer.ReadBitSet();
            this.EmptyBlockLightMask = buffer.ReadBitSet();

            int skyLightCount = buffer.ReadVarInt();
            this.SkyLightArray = new byte[skyLightCount][];
            for (int i = 0; (i < skyLightCount); i++) {
                this.SkyLightArray[i] = buffer.ReadByteArray();
                if (this.SkyLightArray[i].Length != 2048) throw new Exception("Expected 2048 bytes");
            }

            int blockLightCount = buffer.ReadVarInt();
            this.BlockLightArray = new byte[blockLightCount][];
            for (int i = 0; i < blockLightCount; i++) {
                this.BlockLightArray[i] = buffer.ReadByteArray();
                if (this.BlockLightArray[i].Length != 2048) throw new Exception("Expected 2048 bytes");
            }
        }

        public override void Write(PacketBuffer buffer) {
            throw new NotImplementedException();
        }
    }
}

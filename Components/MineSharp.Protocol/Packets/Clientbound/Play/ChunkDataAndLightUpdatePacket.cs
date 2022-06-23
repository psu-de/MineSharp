using fNbt;
using MineSharp.Core.Types;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class ChunkDataAndLightUpdatePacket : Packet {

        public int ChunkX { get; private set; }
        public int ChunkZ { get; private set; }
        public NbtCompound? Heigtmaps { get; private set; }
        public byte[]? Data { get; private set; }
        public BlockEntity[]? BlockEntities { get; private set; }
        public bool TrustEdges { get; private set; }
        public BitSet? SkyLightMask { get; private set; }
        public BitSet? BlockLightMask { get; private set; }
        public BitSet? EmptySkyLightMask { get; private set; }
        public BitSet? EmptyBlockLightMask { get; private set; }
        public byte[][]? SkyLightArray { get; private set; }
        public byte[][]? BlockLightArray { get; private set; }


        public ChunkDataAndLightUpdatePacket() { }

        public ChunkDataAndLightUpdatePacket(int chunkX, int chunkZ, NbtCompound heigtmaps, byte[] data, BlockEntity[] blockEntities, bool trustEdges, BitSet skyLightMask, BitSet blockLightMask, BitSet emptySkyLightMask, BitSet emptyBlockLightMask, byte[][] skyLightArray, byte[][] blockLightArray) {
            ChunkX = chunkX;
            ChunkZ = chunkZ;
            Heigtmaps = heigtmaps;
            Data = data;
            BlockEntities = blockEntities;
            TrustEdges = trustEdges;
            SkyLightMask = skyLightMask;
            BlockLightMask = blockLightMask;
            EmptySkyLightMask = emptySkyLightMask;
            EmptyBlockLightMask = emptyBlockLightMask;
            SkyLightArray = skyLightArray;
            BlockLightArray = blockLightArray;
        }

        public override void Read(PacketBuffer buffer) {
            this.ChunkX = buffer.ReadInt();
            this.ChunkZ = buffer.ReadInt();
            this.Heigtmaps = buffer.ReadNBTCompound();
            this.Data = buffer.ReadByteArray();

            int entityCount = buffer.ReadVarInt();
            this.BlockEntities = new BlockEntity[entityCount];
            for (int i = 0; i < entityCount; i++) {
                byte packedXZ = buffer.ReadByte();

                int x = packedXZ >> 4 & 0xF;
                int z = packedXZ & 0xF;
                short y = buffer.ReadShort();
                int type = buffer.ReadVarInt();
                NbtCompound nbt = buffer.ReadNBTCompound()!;

                this.BlockEntities[i] = new BlockEntity(new Position(x, y, z), (Core.Types.Enums.BlockEntityType)type, nbt);
            }
            this.TrustEdges = buffer.ReadBoolean();
            this.SkyLightMask = buffer.ReadBitSet();
            this.BlockLightMask = buffer.ReadBitSet();
            this.EmptySkyLightMask= buffer.ReadBitSet();
            this.EmptyBlockLightMask= buffer.ReadBitSet();

            int skyLightCount  = buffer.ReadVarInt();
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

using MineSharp.Core.Types;
using MineSharp.Data.Biomes;
using MineSharp.Data.Blocks;
using MineSharp.Protocol.Packets;
using MineSharp.World.PalettedContainer;


namespace MineSharp.World.Chunks {
    public class ChunkSection {

        // Positions used in this Class should be local coordinates, relative to the chunk

        public static ChunkSection Read(PacketBuffer buffer) {
            short solidBlockCount = buffer.ReadShort();
            BlockPalettedContainer blockContainer = BlockPalettedContainer.Read(buffer);
            BiomePalettedContainer biomeContainer = BiomePalettedContainer.Read(buffer);

            return new ChunkSection(solidBlockCount, blockContainer, biomeContainer);
        }


        public short SolidBlockCount { get; private set; }
        private BlockPalettedContainer BlockStorage;
        private BiomePalettedContainer BiomeStorage;

        public ChunkSection(short blockCount, BlockPalettedContainer blockContainer, BiomePalettedContainer biomeContainer) {
            this.SolidBlockCount = blockCount;
            this.BlockStorage = blockContainer;
            this.BiomeStorage = biomeContainer;
        }

        public void Update(long[] blocks) {
            for (int i = 0; i < blocks.Length; i++) {
                int stateId = (int)(blocks[i] >> 12);
                int x = (int)((blocks[i] >> 8) & 0xF);
                int z = (int)((blocks[i] >> 4) & 0xF);
                int y = (int)((blocks[i] >> 0) & 0xF);

                var blockId = BlockPalette.GetBlockIdByState(stateId);
                var block = BlockPalette.CreateBlock(blockId, stateId, new Position(x, y, z));
                this.SetBlock(block);
            }
        }

        public Biome GetBiomeAt(Position pos) {
            return GetBiomeAt(GetBiomeIndex(pos.X, pos.Y, pos.Z));
        }

        public Block GetBlockAt(Position blockPos) {
            int state = BlockStorage.GetAt(GetBlockIndex(blockPos.X, blockPos.Y, blockPos.Z));
            var blockId = BlockPalette.GetBlockIdByState(state);
            var block = BlockPalette.CreateBlock(blockId, state, blockPos);
            return block;
        }

        public void SetBlock(Block block) {
            int index = GetBlockIndex(block.Position!.X, block.Position!.Y, block.Position!.Z);

            var oldBlock = GetBlockAt(block.Position);

            if (oldBlock.IsSolid() && !block.IsSolid()) this.SolidBlockCount--;
            else if (!oldBlock.IsSolid() && block.IsSolid()) this.SolidBlockCount++;


            this.BlockStorage.SetAt(index, (int)block.State!);
        }

        private int GetBiomeIndex(int x, int y, int z) {
            return (y >> 2) << 2 | (z >> 2) << 2 | (x >> 2);
        }

        private int GetBlockIndex(int x, int y, int z) {
            return (y << 8) | (z << 4) | x;
        }

        private Biome GetBiomeAt(int index) {
            var state = BiomeStorage.GetAt(index);
            return BiomePalette.CreateBiome(state);
        }

        public Task<Block?> FindBlockAsync(BlockType type, CancellationToken? cancellation = null) {

            return Task.Factory.StartNew(() => {
                var blockType = BlockPalette.GetBlockTypeById((int)type);
                var searchedBlock = (Block)Activator.CreateInstance(blockType)!;

                if (!this.BlockStorage.Palette.HasState(searchedBlock.MinStateId, searchedBlock.MaxStateId)) return null;

                for (int y = 0; y < Chunk.ChunkSectionLength; y++) {
                    for (int z = 0; z < Chunk.ChunkSectionLength; z++) {
                        for (int x = 0; x < Chunk.ChunkSectionLength; x++) {
                            if (cancellation?.IsCancellationRequested ?? false) return null;
                            int value = BlockStorage.GetAt(GetBlockIndex(x, y, z));
                            if (searchedBlock.MinStateId <= value && value <= searchedBlock.MaxStateId)
                                return GetBlockAt(new Position(x, y, z));
                        }
                    }
                }

                return null;
            });
        }

        public Task<Block[]?> FindBlocksAsync(BlockType type, int count = -1, CancellationToken? cancellation = null) {

            return Task.Factory.StartNew(() => {

                List<Block> blocks = new List<Block>();
                var blockType = BlockPalette.GetBlockTypeById((int)type);
                var searchedBlock = (Block)Activator.CreateInstance(blockType)!;

                if (!this.BlockStorage.Palette.HasState(searchedBlock.MinStateId, searchedBlock.MaxStateId)) return null;

                for (int y = 0; y < Chunk.ChunkSectionLength; y++) {
                    for (int z = 0; z < Chunk.ChunkSectionLength; z++) {
                        for (int x = 0; x < Chunk.ChunkSectionLength; x++) {
                            if (cancellation?.IsCancellationRequested ?? false) return null;
                            int value = BlockStorage.GetAt(GetBlockIndex(x, y, z));
                            if (searchedBlock.MinStateId <= value && value <= searchedBlock.MaxStateId) {
                                blocks.Add(GetBlockAt(new Position(x, y, z)));
                                if (count > 0 && blocks.Count >= count) {
                                    return blocks.Take(count).ToArray();
                                }
                            }
                        }
                    }
                }

                if (blocks.Count == 0) return null;
                return blocks.ToArray();
            });
        }
    }
}

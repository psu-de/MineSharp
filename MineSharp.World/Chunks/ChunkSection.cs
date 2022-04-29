using MineSharp.Core.Types;
using MineSharp.Data.Blocks;
using MineSharp.Protocol.Packets;
using MineSharp.World.PalettedContainer.Palettes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MineSharp.World.Chunks {
    public class ChunkSection {

        public static ChunkSection Read(PacketBuffer buffer) {
            short solidBlockCount = buffer.ReadShort();
            PalettedContainer.PalettedContainer blockContainer = new PalettedContainer.PalettedContainer(false, 16 * 16 * 16);
            PalettedContainer.PalettedContainer biomeContainer = new PalettedContainer.PalettedContainer(true, 4 * 4 * 4);

            blockContainer.Read(buffer);
            biomeContainer.Read(buffer);

            return new ChunkSection(solidBlockCount, blockContainer, biomeContainer);
        }


        public short SolidBlockCount { get; private set; }
        private PalettedContainer.PalettedContainer BlockStorage;
        private PalettedContainer.PalettedContainer BiomeStorage;

        public ChunkSection(short blockCount, PalettedContainer.PalettedContainer blockContainer, PalettedContainer.PalettedContainer biomeContainer) {
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

                this.SetBlock(new Block(BlockData.StateToBlockMap[stateId], new Position(x, y, z), stateId));
            }
        }

        public Block GetBlockAt(Position blockPos) {
            BlockInfo info = GetBlockAt(GetIndex(blockPos.X, blockPos.Y, blockPos.Z), out int state);
            return new Block(info, blockPos, state);
        }

        public void SetBlock(Block block) {
            int index = GetIndex(block.Position.X, block.Position.Y, block.Position.Z);

            var oldBlock = GetBlockAt(block.Position);

            if (oldBlock.IsSolid() && !block.IsSolid()) this.SolidBlockCount--;
            else if (!oldBlock.IsSolid() && block.IsSolid()) this.SolidBlockCount++;


            this.BlockStorage.SetAt(index, block.State);
        }

        private int GetIndex(int x, int y, int z) {
            return (y << 8) | (z << 4) | x;
        }

        private BlockInfo GetBlockAt(int index, out int state) {
            state = BlockStorage.GetAt(index);
            return BlockData.StateToBlockMap[state];
        }

        public Task<Block?> FindBlockAsync(BlockType type, CancellationToken? cancellation = null) {

            return Task.Factory.StartNew(() => {
                var blockInfo = BlockData.Blocks[(int)type];
                if (!this.BlockStorage.Palette.HasState(blockInfo.MinStateId, blockInfo.MaxStateId)) return null;

                for (int y = 0; y < Chunk.ChunkSectionLength; y++) {
                    for (int z = 0; z < Chunk.ChunkSectionLength; z++) {
                        for (int x = 0; x < Chunk.ChunkSectionLength; x++) {
                            if (cancellation?.IsCancellationRequested ?? false) return null;
                            int value = BlockStorage.GetAt(GetIndex(x, y, z));
                            if (blockInfo.MinStateId <= value && value <= blockInfo.MaxStateId)
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

                var blockInfo = BlockData.Blocks[(int)type];
                if (!this.BlockStorage.Palette.HasState(blockInfo.MinStateId, blockInfo.MaxStateId)) return null;

                for (int y = 0; y < Chunk.ChunkSectionLength; y++) {
                    for (int z = 0; z < Chunk.ChunkSectionLength; z++) {
                        for (int x = 0; x < Chunk.ChunkSectionLength; x++) {
                            if (cancellation?.IsCancellationRequested ?? false) return null;
                            int value = BlockStorage.GetAt(GetIndex(x, y, z));
                            if (blockInfo.MinStateId <= value && value <= blockInfo.MaxStateId) {
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

using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using MineSharp.Data.T4.Blocks;
using MineSharp.Protocol.Packets;
using MineSharp.Protocol.Packets.Clientbound.Play;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.World.Chunks {
    public class Chunk {

        private static Logger Logger = Logger.GetLogger();

        public const int ChunkSectionLength = 16;


        public static int GetSectionCount() {
            return World.TotalHeight >> 4;
        }

        public int X { get; set; }
        public int Z { get; set; }
        public ChunkSection[] ChunkSections { get; set; }
        public Dictionary<Position, BlockEntity> BlockEntities { get; set; }


        public Chunk(int x, int z, byte[] data, BlockEntity[] blockEntities) {
            this.X = x;
            this.Z = z;
            this.ChunkSections = new ChunkSection[GetSectionCount()];

            this.Load(data);

            this.BlockEntities = new Dictionary<Position, BlockEntity>();
            foreach (var blockEntity in blockEntities) {
                this.BlockEntities.Add(blockEntity.Position, blockEntity);
            }
        }

        public Chunk(ChunkDataAndLightUpdatePacket packet) : this(packet.ChunkX, packet.ChunkZ, packet.Data, packet.BlockEntities) { }

        public void Load(byte[] data) {
            PacketBuffer buffer = new PacketBuffer(data);

            for (int i = 0; i < this.ChunkSections.Length; i++) {
                this.ChunkSections[i] = ChunkSection.Read(buffer);
            }
        }

        public Position Chunk2WorldPos(Position pos, int sectionIndex) {
            int x = (this.X * ChunkSectionLength) + pos.X;
            int y = ((sectionIndex - 4) * ChunkSectionLength) + pos.Y;
            int z = (this.Z * ChunkSectionLength) + pos.Z;
            return new Position(x, y, z);
        }

        public Position World2ChunkPos(Position pos) {
            int sectionIndex = GetSectionIndex(pos.Y);

            int x = Math.Abs(Math.Abs(pos.X) - Math.Abs(X * ChunkSectionLength));
            int y = Math.Abs(Math.Abs(pos.Y) - Math.Abs((sectionIndex - 4) * ChunkSectionLength));
            int z = Math.Abs(Math.Abs(pos.Z) - Math.Abs(Z * ChunkSectionLength));

            return new Position(x, y, z);
        }

        public void SetBlock(Block block) {
            int sectionIndex = GetSectionIndex(block.Position.Y);
            block.Position = this.World2ChunkPos(block.Position);
            this.ChunkSections[sectionIndex].SetBlock(block);
        }

        public Block GetBlockAt(Position pos) {
            int sectionIndex = GetSectionIndex(pos.Y);
            if (sectionIndex >= this.ChunkSections.Length) throw new Exception("Out of map");
            var chunkPos = this.World2ChunkPos(pos);
            Block block = this.ChunkSections[sectionIndex].GetBlockAt(chunkPos);
            block.Position = pos;
            return block;
        }

        public Biome GetBiomeAt(Position pos) {
            int sectionIndex = GetSectionIndex(pos.Y);
            if (sectionIndex >= this.ChunkSections.Length) throw new Exception("Out of map");
            var chunkPos = this.World2ChunkPos(pos);
            Biome biome = this.ChunkSections[sectionIndex].GetBiomeAt(chunkPos);
            return biome;
        }


        private int GetSectionIndex(int y) {
            return (y - World.MinY) >> 4;
        }

        public async Task<Block?> FindBlockAsync(BlockType type, CancellationToken? cancellation = null) {
            for (int i = 0; i < this.ChunkSections.Length; i++) {
                var block = await this.ChunkSections[i].FindBlockAsync(type, cancellation);

                if (cancellation?.IsCancellationRequested ?? false) return null;

                if (block != null) {
                    block.Position = Chunk2WorldPos(block.Position!, i);
                    return block;
                }
            }
            return null;
        }

        public async Task<Block[]?> FindBlocksAsync(BlockType type, int count = -1, CancellationToken? cancellation = null) {
            List<Block> blocks = new List<Block>();

            for (int i = 0; i < this.ChunkSections.Length; i++) {
                var sectionBlocks = await this.ChunkSections[i].FindBlocksAsync(type, count - blocks.Count, cancellation);
                if (cancellation?.IsCancellationRequested ?? false) return null;

                if (sectionBlocks != null) {
                    sectionBlocks = sectionBlocks.Select(block => { block.Position = Chunk2WorldPos(block.Position, i); return block; }).ToArray();
                    blocks.AddRange(sectionBlocks);
                    if (count > 0 && blocks.Count >= count) {
                        return blocks.Take(count).ToArray();
                    }
                }
            }

            if (blocks.Count == 0) return null;
            return blocks.ToArray();
        }
    }
}

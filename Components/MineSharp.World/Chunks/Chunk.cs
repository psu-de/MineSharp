using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using MineSharp.Data.Blocks;
using MineSharp.Data.Protocol;
using MineSharp.Data.Protocol.Play.Clientbound;

namespace MineSharp.World.Chunks
{
    public class Chunk
    {

        public const int ChunkSectionLength = 16;

        private static Logger Logger = Logger.GetLogger();


        public Chunk(int x, int z, byte[] data, ChunkBlockEntity[] blockEntities)
        {
            this.X = x;
            this.Z = z;
            this.ChunkSections = new ChunkSection[GetSectionCount()];

            this.Load(data);

            this.BlockEntities = new Dictionary<Position, ChunkBlockEntity>();
            foreach (var blockEntity in blockEntities)
            {
                this.BlockEntities.Add(new Position(blockEntity.X, blockEntity.Y, blockEntity.Z), blockEntity);
            }
        }

        public Chunk(PacketMapChunk packet) : this(packet.X, packet.Z, packet.ChunkData!, packet.BlockEntities) {}

        public int X { get; set; }
        public int Z { get; set; }
        public ChunkSection[] ChunkSections { get; set; }
        public Dictionary<Position, ChunkBlockEntity> BlockEntities { get; set; }


        public static int GetSectionCount() => World.TotalHeight >> 4;

        public void Load(byte[] data)
        {
            var buffer = new PacketBuffer(data);

            for (var i = 0; i < this.ChunkSections.Length; i++)
            {
                this.ChunkSections[i] = ChunkSection.Read(buffer);
            }
        }

        public Position Chunk2WorldPos(Position pos, int sectionIndex)
        {
            var x = this.X * ChunkSectionLength + pos.X;
            var y = (sectionIndex - 4) * ChunkSectionLength + pos.Y;
            var z = this.Z * ChunkSectionLength + pos.Z;
            return new Position(x, y, z);
        }

        public Position World2ChunkPos(Position pos)
        {
            var sectionIndex = this.GetSectionIndex(pos.Y);

            var x = Math.Abs(Math.Abs(pos.X) - Math.Abs(this.X * ChunkSectionLength));
            var y = Math.Abs(Math.Abs(pos.Y) - Math.Abs((sectionIndex - 4) * ChunkSectionLength));
            var z = Math.Abs(Math.Abs(pos.Z) - Math.Abs(this.Z * ChunkSectionLength));

            return new Position(x, y, z);
        }

        public void SetBlock(Block block)
        {
            var sectionIndex = this.GetSectionIndex(block.Position.Y);
            block.Position = this.World2ChunkPos(block.Position);
            this.ChunkSections[sectionIndex].SetBlock(block);
        }

        public Block GetBlockAt(Position pos)
        {
            var sectionIndex = this.GetSectionIndex(pos.Y);
            if (sectionIndex >= this.ChunkSections.Length) throw new Exception("Out of map");
            var chunkPos = this.World2ChunkPos(pos);
            var block = this.ChunkSections[sectionIndex].GetBlockAt(chunkPos);
            block.Position = pos;
            return block;
        }

        public Biome GetBiomeAt(Position pos)
        {
            var sectionIndex = this.GetSectionIndex(pos.Y);
            if (sectionIndex >= this.ChunkSections.Length) throw new Exception("Out of map");
            var chunkPos = this.World2ChunkPos(pos);
            var biome = this.ChunkSections[sectionIndex].GetBiomeAt(chunkPos);
            return biome;
        }


        private int GetSectionIndex(int y) => y - World.MinY >> 4;

        public async Task<Block?> FindBlockAsync(BlockType type, CancellationToken? cancellation = null)
        {
            for (var i = 0; i < this.ChunkSections.Length; i++)
            {
                var block = await this.ChunkSections[i].FindBlockAsync(type, cancellation);

                if (cancellation?.IsCancellationRequested ?? false) return null;

                if (block != null)
                {
                    block.Position = this.Chunk2WorldPos(block.Position!, i);
                    return block;
                }
            }
            return null;
        }

        public async Task<Block[]?> FindBlocksAsync(BlockType type, int count = -1, CancellationToken? cancellation = null)
        {
            var blocks = new List<Block>();

            for (var i = 0; i < this.ChunkSections.Length; i++)
            {
                var sectionBlocks = await this.ChunkSections[i].FindBlocksAsync(type, count - blocks.Count, cancellation);
                if (cancellation?.IsCancellationRequested ?? false) return null;

                if (sectionBlocks != null)
                {
                    sectionBlocks = sectionBlocks.Select(block =>
                    {
                        block.Position = this.Chunk2WorldPos(block.Position!, i);
                        return block;
                    }).ToArray();
                    blocks.AddRange(sectionBlocks);
                    if (count > 0 && blocks.Count >= count)
                    {
                        return blocks.Take(count).ToArray();
                    }
                }
            }

            if (blocks.Count == 0) return null;
            return blocks.ToArray();
        }
    }
}

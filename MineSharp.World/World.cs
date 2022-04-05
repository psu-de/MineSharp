using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using MineSharp.Data.Blocks;
using MineSharp.Protocol.Packets;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.World.Chunks;
using System.Collections.Concurrent;

namespace MineSharp.World {
    public class World {

        private static Logger Logger = Logger.GetLogger();

        public const int MaxY = 320;
        public const int MinY = -64;
        public const int TotalHeight = MaxY - MinY;

        public ConcurrentDictionary<(int, int), Chunk> Chunks = new ConcurrentDictionary<(int, int), Chunk>();

        public delegate void BlockEvent(Block block);
        public event BlockEvent BlockUpdated;

        public delegate void ChunkEvent(Chunk chunk);
        public event ChunkEvent ChunkLoaded;
        public event ChunkEvent ChunkUnloaded;

        public World() {

        }

        public void LoadChunkPacket(ChunkDataAndLightUpdatePacket packet) {
            var chunk = this.GetChunkAt(packet.ChunkX, packet.ChunkZ);
            if (chunk == null) {
                chunk = new Chunk(packet);
                if (!this.Chunks.TryAdd((packet.ChunkX, packet.ChunkZ), chunk)) Logger.Error("Bruh");
            } else {
                chunk.Load(packet.Data);
                ChunkLoaded?.Invoke(chunk);
            }
        }

        public void UnloadChunk((int, int) chunk) {
            Chunks.TryRemove(chunk, out var _chunk);
            if (_chunk != null) ChunkUnloaded?.Invoke(_chunk);
        }

        public Chunk? GetChunkAt(int x, int z) {
            Chunk? chunk = null;
            Chunks.TryGetValue((x, z), out chunk);
            return chunk;
        }

        public (int, int) GetChunkCoordinates(int x, int z) {
            int chunkX;
            int chunkZ;
            if (x >= 0) chunkX = x >> 4;
            else chunkX = (int)Math.Floor((float)(x >> 4));


            if (z >= 0) chunkZ = z >> 4;
            else chunkZ = (int)Math.Floor((float)(z >> 4));
            return (chunkX, chunkZ);
        }

        public bool IsBlockLoaded(Position blockPos) {
            (int cX, int cZ) = GetChunkCoordinates(blockPos.X, blockPos.Z);
            return GetChunkAt(cX, cZ) != null;
        }

        public bool IsBlockLoaded(Position blockPos, out Chunk? chunk) {

            (int cX, int cZ) = GetChunkCoordinates(blockPos.X, blockPos.Z);
            chunk = GetChunkAt(cX, cZ);
            return chunk != null;
        }

        public void SetBlock(Block block) {

            (int chunkX, int chunkZ) = GetChunkCoordinates(block.Position.X, block.Position.Z);
            if (!this.IsBlockLoaded(block.Position, out var chunk)) throw new Exception($"Chunk ({chunkX} / {chunkZ}) is not loaded");
            else {
                chunk.SetBlock(block);
                BlockUpdated?.Invoke(block);
            }
        }

        public Block GetBlockAt(Position pos) {
            (int chunkX, int chunkZ) = GetChunkCoordinates(pos.X, pos.Z);

            if (!this.IsBlockLoaded(pos, out var chunk)) throw new Exception($"Chunk ({chunkX} / {chunkZ}) is not loaded");
            else {
                return chunk.GetBlockAt(pos);
            }
        }

        public async Task<Block[]?> FindBlocksAsync(BlockType type, int count = -1) {
            List<Block> blocks = new List<Block>();
            foreach (var chunk in this.Chunks.Values) {
                var chunkBlocks = await chunk.FindBlocksAsync(type, count - blocks.Count);
                if (chunkBlocks != null) {
                    blocks.AddRange(chunkBlocks);
                    if (count > 0 && blocks.Count >= count) {
                        return blocks.Take(count).ToArray();
                    }
                }
            }

            if (blocks.Count == 0) return null;
            return blocks.ToArray();
        }

        public async Task<Block?> FindBlockAsync(BlockType type) {
            foreach (var chunk in this.Chunks.Values) {
                var block = await chunk.FindBlockAsync(type);
                if (block != null) return block;
            }

            return null;
        }
    }
}
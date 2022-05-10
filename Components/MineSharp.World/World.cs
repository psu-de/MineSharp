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

        public ConcurrentDictionary<ChunkCoordinates, Chunk> Chunks = new ConcurrentDictionary<ChunkCoordinates, Chunk>();

        public delegate void BlockEvent(Block block);
        public event BlockEvent BlockUpdated;

        public delegate void ChunkEvent(Chunk chunk);
        public event ChunkEvent ChunkLoaded;
        public event ChunkEvent ChunkUnloaded;

        public World() {

        }

        public void LoadChunkPacket(ChunkDataAndLightUpdatePacket packet) {
            var chunkCoords = new ChunkCoordinates(packet.ChunkX, packet.ChunkZ);
            var chunk = this.GetChunkAt(chunkCoords);
            if (chunk == null) {
                chunk = new Chunk(packet);
                if (this.Chunks.TryAdd(chunkCoords, chunk)) {
                    ChunkLoaded?.Invoke(chunk);
                }
                else Logger.Error("Could not add chunk!");
            } else {
                chunk.Load(packet.Data);
                ChunkLoaded?.Invoke(chunk);
            }
        }

        public void MultiblockUpdate(long[] blocks, int cX, int cY, int cZ) {
            var chunk = this.GetChunkAt(new ChunkCoordinates(cX, cZ));
            if (chunk == null) return;

            foreach (var l in blocks) {
                int blockZ = (int)((l >> 4) & 0x0F);
                int blockX = (int)((l >> 8) & 0x0F);
                int blockY = (int)(l & 0x0F);
                int stateId = (int)(l >> 12);

                var block = new Block(BlockData.StateToBlockMap[stateId], new Position(blockX, blockY, blockZ), stateId);

                chunk.ChunkSections[cY].SetBlock(block);

                block.Position = chunk.Chunk2WorldPos(block.Position, cY);

                BlockUpdated?.Invoke(block);
            }
        }

        public void UnloadChunk(ChunkCoordinates chunk) {
            Chunks.TryRemove(chunk, out var _chunk);
            if (_chunk != null) ChunkUnloaded?.Invoke(_chunk);
        }

        public Chunk? GetChunkAt(ChunkCoordinates coords) {
            Chunk? chunk = null;
            Chunks.TryGetValue(coords, out chunk);
            return chunk;
        }

        public ChunkCoordinates GetChunkCoordinates(int x, int z) {
            int chunkX;
            int chunkZ;
            if (x >= 0) chunkX = x >> 4;
            else chunkX = (int)Math.Floor((float)(x >> 4));


            if (z >= 0) chunkZ = z >> 4;
            else chunkZ = (int)Math.Floor((float)(z >> 4));
            return new ChunkCoordinates(chunkX, chunkZ);
        }

        public bool IsBlockLoaded(Position blockPos) {
            ChunkCoordinates coords = GetChunkCoordinates(blockPos.X, blockPos.Z);
            return GetChunkAt(coords) != null;
        }

        public bool IsBlockLoaded(Position blockPos, out Chunk? chunk) {

            ChunkCoordinates coords = GetChunkCoordinates(blockPos.X, blockPos.Z);
            chunk = GetChunkAt(coords);
            return chunk != null;
        }

        public void SetBlock(Block block) {

            ChunkCoordinates coords = GetChunkCoordinates(block.Position.X, block.Position.Z);
            if (!this.IsBlockLoaded(block.Position, out var chunk)) throw new Exception($"Chunk {coords} is not loaded");
            else {
                chunk.SetBlock(block);
                BlockUpdated?.Invoke(block);
            }
        }
        
        public bool IsOutOfMap(Position pos) {
            if (pos.Y <= World.MinY || pos.Y >= World.MaxY) return true;
            if (Math.Abs(pos.X) >= 29999984) return true;
            if (Math.Abs(pos.Z) >= 29999984) return true;
            return false;
        }

        public Block GetBlockAt(Position pos) {
            if (this.IsOutOfMap(pos)) throw new ArgumentException("Position is out of map");
            ChunkCoordinates coords = GetChunkCoordinates(pos.X, pos.Z);

            if (!this.IsBlockLoaded(pos, out var chunk)) throw new Exception($"Chunk {coords} is not loaded");
            else {
                return chunk.GetBlockAt(pos);
            }
        }

        public async Task<Block[]?> FindBlocksAsync(BlockType type, int count = -1, CancellationToken? cancellation = null) {
            List<Block> blocks = new List<Block>();
            foreach (var chunk in this.Chunks.Values) {
                var chunkBlocks = await chunk.FindBlocksAsync(type, count - blocks.Count, cancellation);

                if (cancellation?.IsCancellationRequested ?? false) return null;

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

        public async Task<Block?> FindBlockAsync(BlockType type, CancellationToken? cancellation = null) {
            foreach (var chunk in this.Chunks.Values) {
                var block = await chunk.FindBlockAsync(type, cancellation);
                if (cancellation?.IsCancellationRequested ?? false) return null; 
                if (block != null) return block;
            }

            return null;
        }
    }
}
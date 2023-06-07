using MineSharp.Core.Common;
using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Blocks;
using MineSharp.World.Chunks;
using System.Diagnostics.CodeAnalysis;

namespace MineSharp.World;

public interface IWorld
{
    public int MaxY { get; }
    public int MinY { get; }
    public int TotalHeight => MaxY - MinY;

    public event Events.ChunkEvent OnChunkLoaded;
    public event Events.ChunkEvent OnChunkUnloaded;
    public event Events.BlockEvent OnBlockUpdated;

    public IChunk GetChunkAt(ChunkCoordinates coordinates);
    public bool IsChunkLoaded(ChunkCoordinates coordinates);
    public void LoadChunk(IChunk chunk);
    public void UnloadChunk(ChunkCoordinates coordinates);

    public bool IsOutOfMap(Position position);
    
    public bool IsBlockLoaded(Position position, [NotNullWhen(true)] out IChunk? chunk);
    public bool IsBlockLoaded(Position position) => IsBlockLoaded(position, out _);

    public Block GetBlockAt(Position position);
    public void SetBlock(Block block);

    public Biome GetBiomeAt(Position position);
    public void SetBiomeAt(Position position, Biome biome);

    public IEnumerable<Block> FindBlocks(int blockId, int? maxCount = null);
    public Block? FindBlock(int blockId) => FindBlocks(blockId).FirstOrDefault();
}

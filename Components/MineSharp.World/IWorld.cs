using System.Diagnostics.CodeAnalysis;
using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Events;
using MineSharp.Core.Geometry;
using MineSharp.World.Chunks;
using MineSharp.World.Iterators;

namespace MineSharp.World;

/// <summary>
///     Interface for creating a minecraft world
/// </summary>
public interface IWorld
{
    /// <summary>
    ///     The max Y coordinate (build height)
    /// </summary>
    public int MaxY { get; }

    /// <summary>
    ///     The min Y coordinate
    /// </summary>
    public int MinY { get; }

    /// <summary>
    ///     The height of the world
    /// </summary>
    public int TotalHeight => MaxY - MinY;

    /// <summary>
    ///     Event fired when a chunk loaded
    /// </summary>
    public AsyncEvent<IWorld, IChunk> OnChunkLoaded { get; set; }

    /// <summary>
    ///     Fired when a was unloaded
    /// </summary>
    public AsyncEvent<IWorld, IChunk> OnChunkUnloaded { get; set; }

    /// <summary>
    ///     Fired when a block was updated
    /// </summary>
    public AsyncEvent<IWorld, Block> OnBlockUpdated { get; set; }

    /// <summary>
    ///     Converts a World position to chunk coordinates
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public ChunkCoordinates ToChunkCoordinates(Position position);

    /// <summary>
    ///     Convert a world position to a position relative to a chunk
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Position ToChunkPosition(Position position);

    /// <summary>
    ///     Convert a position relative to the given chunk coordinates to the world position
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public Position ToWorldPosition(ChunkCoordinates coordinates, Position position);

    /// <summary>
    ///     Return the chunk at the given chunk position
    /// </summary>
    /// <param name="coordinates"></param>
    /// <returns></returns>
    public IChunk GetChunkAt(ChunkCoordinates coordinates);

    /// <summary>
    ///     Try to get the chunk at the given chunk coordinates.
    ///     This method does the same as <see cref="GetChunkAt" /> but does not throw an exception.
    ///     Instead it returns a boolean indicating the success of the operation.
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="chunk"></param>
    /// <returns></returns>
    public bool TryGetChunkAt(ChunkCoordinates coordinates, [NotNullWhen(true)] out IChunk? chunk);

    /// <summary>
    ///     Whether the chunk at the given coordinates is loaded
    /// </summary>
    /// <param name="coordinates"></param>
    /// <returns></returns>
    public bool IsChunkLoaded(ChunkCoordinates coordinates);

    /// <summary>
    ///     Load a chunk
    /// </summary>
    /// <param name="chunk"></param>
    public void LoadChunk(IChunk chunk);

    /// <summary>
    ///     Unload a chunk from the world
    /// </summary>
    /// <param name="coordinates"></param>
    public void UnloadChunk(ChunkCoordinates coordinates);

    /// <summary>
    ///     Create a new chunk at the given coordinates.
    ///     But don't load it yet.
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="entities"></param>
    /// <returns></returns>
    public IChunk CreateChunk(ChunkCoordinates coordinates, BlockEntity[] entities);

    /// <summary>
    ///     Whether the position is out of the world
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool IsOutOfMap(Position position);

    /// <summary>
    ///     Whether the position is loaded (chunk at position is loaded)
    /// </summary>
    /// <param name="position"></param>
    /// <param name="chunk"></param>
    /// <returns></returns>
    public bool IsBlockLoaded(Position position, [NotNullWhen(true)] out IChunk? chunk);

    /// <summary>
    ///     Whether the position is loaded (chunk at position is loaded)
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool IsBlockLoaded(Position position)
    {
        return IsBlockLoaded(position, out _);
    }

    /// <summary>
    ///     Return the block at the given position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Block GetBlockAt(Position position);

    /// <summary>
    ///     Set the block at the given position
    /// </summary>
    /// <param name="block"></param>
    public void SetBlock(Block block);

    /// <summary>
    ///     Get the biome at the given position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Biome GetBiomeAt(Position position);

    /// <summary>
    ///     Set the biome at the given position
    /// </summary>
    /// <param name="position"></param>
    /// <param name="biome"></param>
    public void SetBiomeAt(Position position, Biome biome);

    /// <summary>
    ///     Query the world for a block type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="iterator"></param>
    /// <param name="maxCount"></param>
    /// <returns></returns>
    public IEnumerable<Block> FindBlocks(BlockType type, IWorldIterator iterator, int? maxCount = null);

    /// <summary>
    ///     Find a block of type <paramref name="type" />
    /// </summary>
    /// <param name="type"></param>
    /// <param name="iterator"></param>
    /// <returns></returns>
    public Block? FindBlock(BlockType type, IWorldIterator iterator)
    {
        return FindBlocks(type, iterator).FirstOrDefault();
    }

    /// <summary>
    ///     Search through all loaded chunks for the block type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="maxCount"></param>
    /// <returns></returns>
    [Obsolete]
    public IEnumerable<Block> FindBlocks(BlockType type, int? maxCount = null);

    /// <summary>
    ///     Search through all loaded chunks for the block type and return the first result.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [Obsolete]
    public Block? FindBlock(BlockType type)
    {
        return FindBlocks(type).FirstOrDefault();
    }
}

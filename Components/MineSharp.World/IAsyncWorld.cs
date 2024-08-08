using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Geometry;
using MineSharp.World.Chunks;
using MineSharp.World.Iterators;

namespace MineSharp.World;

/// <summary>
///     Interface for creating a Minecraft world
/// </summary>
public interface IAsyncWorld : IWorld
{
    /// <summary>
    ///     Return the chunk at the given chunk position.
    ///     Waits for the chunk to be loaded if it's not loaded yet.
    /// </summary>
    /// <param name="coordinates"></param>
    /// <returns></returns>
    public Task<IChunk> GetChunkAtAsync(ChunkCoordinates coordinates);

    /// <summary>
    ///     Return the block at the given position.
    ///     Waits for the chunk the block is in to be loaded if it's not loaded yet.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Task<Block> GetBlockAtAsync(Position position);

    /// <summary>
    ///     Set the block at the given position.
    ///     Waits for the chunk the block is in to be loaded if it's not loaded yet.
    /// </summary>
    /// <param name="block"></param>
    public Task SetBlockAsync(Block block);

    /// <summary>
    ///     Get the biome at the given position.
    ///     Waits for the chunk the block is in to be loaded if it's not loaded yet.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Task<Biome> GetBiomeAtAsync(Position position);

    /// <summary>
    ///     Set the biome at the given position.
    ///     Waits for the chunk the block is in to be loaded if it's not loaded yet.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="biome"></param>
    public Task SetBiomeAtAsync(Position position, Biome biome);

    /// <summary>
    ///     Query the world for a block type.
    ///     Waits for the chunks the block are in to be loaded if they are not loaded yet.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="iterator"></param>
    /// <param name="maxCount"></param>
    /// <returns></returns>
    public IAsyncEnumerable<Block> FindBlocksAsync(BlockType type, IWorldIterator iterator, int? maxCount = null);

    /// <summary>
    ///     Find a block of type <paramref name="type" />.
    ///     Waits for the chunks the block are in to be loaded if they are not loaded yet.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="iterator"></param>
    /// <returns></returns>
    public async Task<Block?> FindBlockAsync(BlockType type, IWorldIterator iterator)
    {
        await foreach (var block in FindBlocksAsync(type, iterator, 1))
        {
            return block;
        }
        return null;
    }
}

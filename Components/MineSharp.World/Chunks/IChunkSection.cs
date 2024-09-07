using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Geometry;

namespace MineSharp.World.Chunks;

/// <summary>
///     Interface for creating a chunk section.
///     A chunk section is an 16x16x16 area in a chunk
/// </summary>
public interface IChunkSection
{
    /// <summary>
    ///     The number of solid blocks in this chunk section.
    /// </summary>
    public short SolidBlockCount { get; }

    /// <summary>
    ///     Returns the Block at the given position.
    ///     Position is considered to be relative to the chunk section.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public int GetBlockAt(Position position);


    /// <summary>
    ///     Set the block state at the given position.
    /// </summary>
    /// <param name="state"></param>
    /// <param name="position">Position must be relative to the chunk section</param>
    public void SetBlockAt(int state, Position position);

    /// <summary>
    ///     Get the biome at the given position
    /// </summary>
    /// <param name="position">Position must be relative to the chunk section</param>
    /// <returns></returns>
    public Biome GetBiomeAt(Position position);

    /// <summary>
    ///     Set the biome at the given position
    /// </summary>
    /// <param name="position">Position must be relative to the chunk section</param>
    /// <param name="biome"></param>
    public void SetBiomeAt(Position position, Biome biome);

    /// <summary>
    ///     Search through the chunk section for the given block type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="maxCount"></param>
    /// <returns></returns>
    [Obsolete]
    public IEnumerable<Block> FindBlocks(BlockType type, int? maxCount = null);

    /// <summary>
    ///     Search through the chunk section for the given block type and return the first block.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [Obsolete]
    public Block? FindBlock(BlockType type)
    {
        return FindBlocks(type).FirstOrDefault();
    }
}

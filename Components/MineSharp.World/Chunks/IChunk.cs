using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Events;
using MineSharp.Core.Geometry;

namespace MineSharp.World.Chunks;

/// <summary>
///     Interface for implementing a chunk.
///     A chunk is an 16x16xWorldHeight area in the world.
/// </summary>
public interface IChunk
{
    /// <summary>
    ///     The size (X, Z direction) of a chunk
    /// </summary>
    public const int Size = 16;

    /// <summary>
    ///     The XZ Coordinates of this chunk.
    /// </summary>
    public ChunkCoordinates Coordinates { get; }

    /// <summary>
    ///     Fired whenever a block in the chunk was updated
    /// </summary>
    public AsyncEvent<IChunk, int, Position> OnBlockUpdated { get; set; }

    /// <summary>
    ///     Loads the chunk data from raw bytes.
    /// </summary>
    /// <param name="data"></param>
    public void LoadData(byte[] data);

    /// <summary>
    ///     Returns the block entity at the given position or null if no
    ///     block entity exists at the position.
    ///     Position is considered to be a relative position in the chunk,
    ///     not absolute in the world.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public BlockEntity? GetBlockEntity(Position position);

    /// <summary>
    ///     Returns the block state at the given position.
    ///     Position is considered to be a relative position in the chunk,
    ///     not absolute in the world.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public int GetBlockAt(Position position);


    /// <summary>
    ///     Sets the block state at the given position.
    ///     Position is expected to be relative to the chunk.
    /// </summary>
    /// <param name="state"></param>
    /// <param name="position"></param>
    public void SetBlockAt(int state, Position position);

    /// <summary>
    ///     Returns the biome of the given position.
    ///     Position is considered to be a relative position in the chunk,
    ///     not absolute in the world.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Biome GetBiomeAt(Position position);

    /// <summary>
    ///     Sets the biome of the at the given position
    ///     Position is considered to be a relative position in the chunk,
    ///     not absolute in the world.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="biome"></param>
    public void SetBiomeAt(Position position, Biome biome);

    /// <summary>
    ///     Search through the chunk for the given block type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="maxCount"></param>
    /// <returns></returns>
    [Obsolete]
    public IEnumerable<Block> FindBlocks(BlockType type, int? maxCount = null);

    /// <summary>
    ///     Search through the chunk for the given block type and return the first block.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [Obsolete]
    public Block? FindBlock(BlockType type)
    {
        return FindBlocks(type).FirstOrDefault();
    }
}

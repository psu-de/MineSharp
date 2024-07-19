namespace MineSharp.World.New.Chunks.Sections;

/// <summary>
/// Interface to implement a chunk section
/// </summary>
public interface IChunkSection
{
    /// <summary>
    ///     The number of solid blocks in this chunk section.
    /// </summary>
    public short SolidBlockCount { get; protected set; }

    /// <summary>
    ///     Returns the Block at the coordinates.
    ///     The coordinates are relative to the chunk.
    /// </summary>
    public int GetBlockStateAt(int x, int y, int z);
    
    /// <summary>
    ///     Set the block state at the coordinates.
    ///     The coordinates are relative to the chunk.
    /// </summary>
    public void SetBlockStateAt(int state, int x, int y, int z);

    /// <summary>
    ///     Get the biome at the given coordinates.
    ///     The coordinates are relative to the chunk
    /// </summary>
    public int GetBiomeStateAt(int x, int y, int z);

    /// <summary>
    ///     Set the biome at the given coordinates.
    ///     The coordinates are relative to the chunk
    /// </summary>
    public void SetBiomeStateAt(int state, int x, int y, int z);
}

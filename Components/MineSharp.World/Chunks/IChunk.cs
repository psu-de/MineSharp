using MineSharp.Core.Common;
using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Blocks;

namespace MineSharp.World.Chunks;

public interface IChunk
{
    public const int SIZE = 16;
    
    public event Events.ChunkBlockEvent OnBlockUpdated;
    
    /// <summary>
    /// The XZ Coordinates of this chunk.
    /// </summary>
    public ChunkCoordinates Coordinates { get; }

    /// <summary>
    /// Loads the chunk data from raw bytes.
    /// </summary>
    /// <param name="data"></param>
    public void LoadData(byte[] data);
    
    /// <summary>
    /// Returns the block entity at the given position or null if no
    /// block entity exists at the position.
    ///
    /// Position is considered to be a relative position in the chunk,
    /// not absolute in the world.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public BlockEntity? GetBlockEntity(Position position);

    /// <summary>
    /// Returns the block state at the given position.
    /// 
    /// Position is considered to be a relative position in the chunk,
    /// not absolute in the world.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public int GetBlockAt(Position position);
    

    /// <summary>
    /// Sets the block state at the given position.
    ///
    /// Position is expected to be relative to the chunk.
    /// </summary>
    /// <param name="state"></param>
    /// <param name="position"></param>
    public void SetBlockAt(int state, Position position);

    /// <summary>
    /// Returns the biome of the given position.
    ///
    /// Position is considered to be a relative position in the chunk,
    /// not absolute in the world.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Biome GetBiomeAt(Position position);
    
    /// <summary>
    /// Sets the biome of the at the given position
    ///
    /// Position is considered to be a relative position in the chunk,
    /// not absolute in the world.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="biome"></param>
    public void SetBiomeAt(Position position, Biome biome);

    [Obsolete]
    public IEnumerable<Block> FindBlocks(BlockType type, int? maxCount = null);
    [Obsolete]
    public Block? FindBlock(BlockType type) => FindBlocks(type).FirstOrDefault();
}

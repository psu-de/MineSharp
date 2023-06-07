using MineSharp.Core.Common;
using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Blocks;

namespace MineSharp.World.Chunks;

public interface IChunk
{
    /// <summary>
    /// The XZ Coordinates of this chunk.
    /// </summary>
    public ChunkCoordinates Coordinates { get; }
    
    /// <summary>
    /// The size of this chunk.
    /// </summary>
    public int Size { get; }

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
    /// Returns the block at the given position.
    /// 
    /// Position is considered to be a relative position in the chunk,
    /// not absolute in the world.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Block GetBlockAt(Position position);
    
    /// <summary>
    /// Sets the block at the given position.
    ///
    /// It is expected that block.Position is a relative position in the chunk,
    /// not absolute in the world.
    /// </summary>
    /// <param name="block"></param>
    public void SetBlock(Block block);

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

    public IEnumerable<Block> FindBlocks(int blockId, int? maxCount = null);
    public Block? FindBlock(int blockId) => FindBlocks(blockId).FirstOrDefault();
}

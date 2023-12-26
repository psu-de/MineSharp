using MineSharp.Core.Common;
using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Blocks;

namespace MineSharp.World.Chunks;

public interface IChunkSection
{
    /// <summary>
    /// The number of solid blocks in this chunk section.
    /// </summary>
    public short SolidBlockCount { get; protected set; }

    /// <summary>
    /// Returns the Block at the given position.
    /// Position is considered to be relative to the chunk section.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public int GetBlockAt(Position position);
    
    
    public void SetBlockAt(int state, Position position);

    public Biome GetBiomeAt(Position position);
    public void SetBiomeAt(Position position, Biome biome);

    [Obsolete]
    public IEnumerable<Block> FindBlocks(BlockType type, int? maxCount = null);
    [Obsolete]
    public Block? FindBlock(BlockType type) => FindBlocks(type).FirstOrDefault();
}

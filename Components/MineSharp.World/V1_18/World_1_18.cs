using MineSharp.Core.Common;
using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Blocks;
using MineSharp.Data;
using MineSharp.World.Chunks;
using MineSharp.World.Exceptions;
using MineSharp.World.Iterators;
using NLog;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace MineSharp.World.V1_18;

public class World_1_18 : AbstractWorld
{
    public const int WORLD_HEIGHT = MAX_Y - MIN_Y;
    public const int MIN_Y = -64;
    public const int MAX_Y = 320;

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger(typeof(IWorld));

    public override int MaxY => MAX_Y;
    public override int MinY => MIN_Y;
    
    public World_1_18(MinecraftData data) : base(data)
    { }

    public override bool IsOutOfMap(Position position)
    {
        if (position.Y <= MinY || position.Y >= MaxY) 
            return true;
        
        if (Math.Abs(position.X) >= 29999984) 
            return true;
        
        if (Math.Abs(position.Z) >= 29999984) 
            return true;
        
        return false;
    }

    public override IChunk CreateChunk(ChunkCoordinates coordinates, BlockEntity[] entities)
    {
        return new Chunk_1_18(this.Data, coordinates, entities);
    }

    public IEnumerable<Block> FindBlocks(BlockType type, IWorldIterator iterator, int? maxCount = null)
    {
        int count = 0;
        foreach (var pos in iterator.Iterate())
        {
            // TODO: Don't use GetBlockAt(), check the palette container directly since most of the time its not necessary to create a new block
            var block = this.GetBlockAt(pos);
            if (block.Info.Type != type)
                continue;

            yield return block;
            
            if (++count == maxCount)
                yield break;
        }
    }
    
    public IEnumerable<Block> FindBlocks(BlockType type, int? maxCount = null)
    {
        int found = 0;
        foreach (var chunk in this.Chunks.Values)
        {
            foreach (var block in chunk.FindBlocks(type, maxCount - found))
            {
                block.Position = this.ToWorldPosition(chunk.Coordinates, block.Position);
                yield return block;

                found++;
                if (found >= maxCount)
                {
                    yield  break;
                }
            }
        }
    }
}

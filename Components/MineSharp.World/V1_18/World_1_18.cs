using MineSharp.Core.Common;
using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Blocks;
using MineSharp.World.Chunks;
using MineSharp.World.Exceptions;
using NLog;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace MineSharp.World.V1_18;

public class World_1_18 : IWorld
{
    public const int WORLD_HEIGHT = MAX_Y - MIN_Y;
    public const int MIN_Y = -64;
    public const int MAX_Y = 320;

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger(typeof(IWorld));

    public int MaxY => MIN_Y;
    public int MinY => MAX_Y;
    
    public event Events.ChunkEvent? OnChunkLoaded;
    public event Events.ChunkEvent? OnChunkUnloaded;
    public event Events.BlockEvent? OnBlockUpdated;

    private readonly IDictionary<ChunkCoordinates, IChunk> _chunks;

    public World_1_18()
    {
        this._chunks = new ConcurrentDictionary<ChunkCoordinates, IChunk>();
    }

    public void LoadChunk(IChunk chunk)
    {
        if (this.IsChunkLoaded(chunk.Coordinates))
        {
            //Logger.Debug($"Updating chunk {chunk.Coordinates}.");
            this._chunks[chunk.Coordinates] = chunk;
        }
        else
        {
            this._chunks.Add(chunk.Coordinates, chunk);
        }
        
        OnChunkLoaded?.Invoke(this, chunk);
    }

    public void UnloadChunk(ChunkCoordinates coordinates)
    {
        if (!this._chunks.Remove(coordinates, out var chunk))
        {
            Logger.Warn($"Trying to unload chunk which was not loaded {coordinates}.");
            return;
        }
        
        this.OnChunkUnloaded?.Invoke(this, chunk);
    }

    public bool IsChunkLoaded(ChunkCoordinates coordinates)
    {
        return this._chunks.ContainsKey(coordinates);
    }

    public IChunk GetChunkAt(ChunkCoordinates coordinates)
    {
        if (!this._chunks.TryGetValue(coordinates, out var chunk))
        {
            throw new ChunkNotLoadedException($"The chunk at {coordinates} is not loaded.");
        }

        return chunk;
    }

    public bool IsOutOfMap(Position position)
    {
        if (position.Y <= MinY || position.Y >= MaxY) 
            return true;
        
        if (Math.Abs(position.X) >= 29999984) 
            return true;
        
        if (Math.Abs(position.Z) >= 29999984) 
            return true;
        
        return false;
    }

    public bool IsBlockLoaded(Position position, [NotNullWhen(true)] out IChunk? chunk)
    {
        return this._chunks.TryGetValue(ToChunkCoordinates(position), out chunk);
    }

    public Block GetBlockAt(Position position)
    {
        if (!IsBlockLoaded(position, out var chunk))
        {
            throw new ChunkNotLoadedException($"Block at {position} is not loaded.");
        }

        var relative = WorldToChunkPosition(position);
        var block = chunk.GetBlockAt(relative);
        block.Position = position;
        return block;
    }
    
    public void SetBlock(Block block)
    {
        if (!IsBlockLoaded(block.Position, out var chunk))
        {
            throw new ChunkNotLoadedException($"Block at {block.Position} is not loaded.");
        }
        
        var relative = WorldToChunkPosition(block.Position);
        chunk.SetBlock(new Block(block.Info, block.State, relative));
        this.OnBlockUpdated?.Invoke(this, block);
    }

    public Biome GetBiomeAt(Position position)
    {
        if (!IsBlockLoaded(position, out var chunk))
        {
            throw new ChunkNotLoadedException($"Position {position} is not loaded.");
        }

        var relative = WorldToChunkPosition(position);
        return chunk.GetBiomeAt(relative);
    }
    
    public void SetBiomeAt(Position position, Biome biome)
    {
        if (!IsBlockLoaded(position, out var chunk))
        {
            throw new ChunkNotLoadedException($"Position {position} is not loaded.");
        }
        
        var relative = WorldToChunkPosition(position);
        chunk.SetBiomeAt(relative, biome);
    }

    public IEnumerable<Block> FindBlocks(int blockId, int? maxCount = null)
    {
        int found = 0;
        foreach (var chunk in this._chunks.Values)
        {
            foreach (var block in chunk.FindBlocks(blockId, maxCount - found))
            {
                block.Position = ChunkToWorldPosition(block.Position, chunk.Coordinates);
                yield return block;

                found++;
                if (found >= maxCount)
                {
                    yield  break;
                }
            }
        }
    }

    private ChunkCoordinates ToChunkCoordinates(Position position)
    {
        int chunkX = position.X / ChunkSection_1_18.SECTION_SIZE;
        int chunkZ = position.Z / ChunkSection_1_18.SECTION_SIZE;

        return new ChunkCoordinates(chunkX, chunkZ);
    }

    private Position WorldToChunkPosition(Position position)
    {
        return new Position(
            Mod(position.X, ChunkSection_1_18.SECTION_SIZE),
            position.Y,
            Mod(position.Z, ChunkSection_1_18.SECTION_SIZE));
    }

    private Position ChunkToWorldPosition(Position position, ChunkCoordinates coordinates)
    {
        int dx = coordinates.X * ChunkSection_1_18.SECTION_SIZE;
        int dz = coordinates.Z * ChunkSection_1_18.SECTION_SIZE;

        return new Position(position.X + dx, position.Y, position.Z + dz);
    }

    private int Mod(int x, int m)
    {
        int v = x % m;
        if (v < 0)
            v += m;
        
        return v;
    }
}

using MineSharp.Core.Common;
using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Blocks;
using MineSharp.Data;
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

    private readonly MinecraftData _data;
    private readonly IDictionary<ChunkCoordinates, IChunk> _chunks;

    public World_1_18(MinecraftData data)
    {
        this._data = data;
        this._chunks = new ConcurrentDictionary<ChunkCoordinates, IChunk>();
    }

    public void LoadChunk(IChunk chunk)
    {
        if (this.IsChunkLoaded(chunk.Coordinates))
        {
            var old = this._chunks[chunk.Coordinates];
            old.OnBlockUpdated -= this.OnChunkBlockUpdate;
            
            //Logger.Debug($"Updating chunk {chunk.Coordinates}.");
            this._chunks[chunk.Coordinates] = chunk;
        }
        else
        {
            this._chunks.Add(chunk.Coordinates, chunk);
        }

        chunk.OnBlockUpdated += this.OnChunkBlockUpdate;
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

        var relative = this.ToChunkPosition(position);
        var blockState = chunk.GetBlockAt(relative);
        var block = new Block(
            this._data.Blocks.GetByState(blockState),
            blockState,
            position);
        return block;
    }
    
    public void SetBlock(Block block)
    {
        if (!IsBlockLoaded(block.Position, out var chunk))
        {
            throw new ChunkNotLoadedException($"Block at {block.Position} is not loaded.");
        }
        
        var relative = this.ToChunkPosition(block.Position);
        chunk.SetBlockAt(block.State, relative);
    }

    public Biome GetBiomeAt(Position position)
    {
        if (!IsBlockLoaded(position, out var chunk))
        {
            throw new ChunkNotLoadedException($"Position {position} is not loaded.");
        }

        var relative = this.ToChunkPosition(position);
        return chunk.GetBiomeAt(relative);
    }
    
    public void SetBiomeAt(Position position, Biome biome)
    {
        if (!IsBlockLoaded(position, out var chunk))
        {
            throw new ChunkNotLoadedException($"Position {position} is not loaded.");
        }
        
        var relative = this.ToChunkPosition(position);
        chunk.SetBiomeAt(relative, biome);
    }

    public IEnumerable<Block> FindBlocks(int blockId, int? maxCount = null)
    {
        int found = 0;
        foreach (var chunk in this._chunks.Values)
        {
            foreach (var block in chunk.FindBlocks(blockId, maxCount - found))
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

    public ChunkCoordinates ToChunkCoordinates(Position position)
    {
        int chunkX = (int)Math.Floor((float)position.X / ChunkSection_1_18.SECTION_SIZE);
        int chunkZ = (int)Math.Floor((float)position.Z / ChunkSection_1_18.SECTION_SIZE);

        return new ChunkCoordinates(chunkX, chunkZ);
    }

    private Position ToChunkPosition(Position position)
    {
        return new Position(
            Mod(position.X, ChunkSection_1_18.SECTION_SIZE),
            position.Y,
            Mod(position.Z, ChunkSection_1_18.SECTION_SIZE));
    }

    public Position ToWorldPosition(ChunkCoordinates coordinates, Position position)
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

    private void OnChunkBlockUpdate(IChunk chunk, int state, Position position)
    {
        var worldPosition = this.ToWorldPosition(chunk.Coordinates, position);
        this.OnBlockUpdated?.Invoke(this, new Block(
            this._data.Blocks.GetByState(state), state, worldPosition));
    }
}

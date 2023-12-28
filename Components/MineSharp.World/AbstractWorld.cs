using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Blocks;
using MineSharp.Data;
using MineSharp.World.Chunks;
using MineSharp.World.Exceptions;
using MineSharp.World.Iterators;
using NLog;

namespace MineSharp.World;

/// <summary>
/// Base class for other implementations of the IWorld interface
/// </summary>
/// <param name="data"></param>
public abstract class AbstractWorld(MinecraftData data) : IWorld
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    
    /// <inheritdoc />
    public abstract int MaxY { get; }
    
    /// <inheritdoc />
    public abstract int MinY { get; }
    
    
    /// <inheritdoc />
    public event Events.ChunkEvent? OnChunkLoaded;
    
    /// <inheritdoc />
    public event Events.ChunkEvent? OnChunkUnloaded;
    
    /// <inheritdoc />
    public event Events.BlockEvent? OnBlockUpdated;

    /// <summary>
    /// A dictionary with all loaded chunks
    /// </summary>
    protected ConcurrentDictionary<ChunkCoordinates, IChunk> Chunks
        = new ConcurrentDictionary<ChunkCoordinates, IChunk>();

    /// <summary>
    /// The MinecraftData instance used for this world
    /// </summary>
    public readonly MinecraftData Data = data;
    
    private readonly BlockInfo OutOfMapBlock = data.Blocks.GetByType(BlockType.Air);
    
    /// <inheritdoc />
    [Pure]
    public ChunkCoordinates ToChunkCoordinates(Position position)
    {
        var chunkX = (int)Math.Floor((float)position.X / IChunk.SIZE);
        var chunkZ = (int)Math.Floor((float)position.Z / IChunk.SIZE);

        return new ChunkCoordinates(chunkX, chunkZ);
    }

    /// <inheritdoc />
    [Pure]
    public Position ToChunkPosition(Position position)
    {
        return new Position(
            NonNegativeMod(position.X, IChunk.SIZE),
            position.Y,
            NonNegativeMod(position.Z, IChunk.SIZE));
    }

    /// <inheritdoc />
    [Pure]
    public Position ToWorldPosition(ChunkCoordinates coordinates, Position position)
    {
        var dx = coordinates.X * IChunk.SIZE;
        var dz = coordinates.Z * IChunk.SIZE;

        return new Position(position.X + dx, position.Y, position.Z + dz);
    }

    /// <summary>
    /// Mutate <paramref name="position"/> to a world position
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="position"></param>
    protected void MutateToWorldPosition(ChunkCoordinates coordinates, MutablePosition position)
    {
        var dx = coordinates.X * IChunk.SIZE;
        var dz = coordinates.Z * IChunk.SIZE;
        position.Set(position.X + dx, position.Y, position.Z + dz);
    }

    /// <inheritdoc />
    public IChunk GetChunkAt(ChunkCoordinates coordinates)
    {
        if (!this.Chunks.TryGetValue(coordinates, out var chunk))
        {
            throw new ChunkNotLoadedException($"The chunk at {coordinates} is not loaded.");
        }

        return chunk;
    }

    /// <inheritdoc />
    [Pure]
    public bool IsChunkLoaded(ChunkCoordinates coordinates)
        => this.IsChunkLoaded(coordinates, out _);

    /// <inheritdoc />
    [Pure]
    protected bool IsChunkLoaded(ChunkCoordinates coordinates, [NotNullWhen(true)] out IChunk? chunk)
        => this.Chunks.TryGetValue(coordinates, out chunk);

    /// <inheritdoc />
    public void LoadChunk(IChunk chunk)
    {
        if (this.IsChunkLoaded(chunk.Coordinates, out var oldChunk))
        {
            oldChunk.OnBlockUpdated -= this.OnChunkBlockUpdate;
            this.Chunks[chunk.Coordinates] = chunk;
        } else this.Chunks.TryAdd(chunk.Coordinates, chunk);

        chunk.OnBlockUpdated += this.OnChunkBlockUpdate;
        this.OnChunkLoaded?.Invoke(this, chunk);
    }

    /// <inheritdoc />
    public void UnloadChunk(ChunkCoordinates coordinates)
    {
        if (!this.Chunks.TryRemove(coordinates, out var chunk))
        {
            Logger.Warn($"Trying to unload chunk which was not loaded {coordinates}.");
            return;
        }
        
        this.OnChunkUnloaded?.Invoke(this, chunk);
    }

    /// <inheritdoc />
    public abstract IChunk CreateChunk(ChunkCoordinates coordinates, BlockEntity[] entities);

    /// <inheritdoc />
    public abstract bool IsOutOfMap(Position position);

    /// <inheritdoc />
    public bool IsBlockLoaded(Position position, [NotNullWhen(true)] out IChunk? chunk)
        => this.Chunks.TryGetValue(ToChunkCoordinates(position), out chunk);

    /// <inheritdoc />
    public Block GetBlockAt(Position position)
    {
        if (IsOutOfMap(position))
            return new Block(OutOfMapBlock, OutOfMapBlock.DefaultState, position);
        
        if (!IsBlockLoaded(position, out var chunk))
        {
            throw new ChunkNotLoadedException($"Block at {position} is not loaded.");
        }

        var relative = this.ToChunkPosition(position);
        var blockState = chunk.GetBlockAt(relative);
        var block = new Block(
            this.Data.Blocks.GetByState(blockState),
            blockState,
            position);
        return block;
    }

    /// <inheritdoc />
    public void SetBlock(Block block)
    {
        if (!this.IsBlockLoaded(block.Position, out var chunk))
        {
            throw new ChunkNotLoadedException($"Block at {block.Position} is not loaded.");
        }
        
        var relative = this.ToChunkPosition(block.Position);
        chunk.SetBlockAt(block.State, relative);
    }

    /// <inheritdoc />
    public Biome GetBiomeAt(Position position)
    {
        if (!this.IsBlockLoaded(position, out var chunk))
        {
            throw new ChunkNotLoadedException($"Position {position} is not loaded.");
        }

        var relative = this.ToChunkPosition(position);
        return chunk.GetBiomeAt(relative);
    }

    /// <inheritdoc />
    public void SetBiomeAt(Position position, Biome biome)
    {
        if (!this.IsBlockLoaded(position, out var chunk))
        {
            throw new ChunkNotLoadedException($"Position {position} is not loaded.");
        }
        
        var relative = this.ToChunkPosition(position);
        chunk.SetBiomeAt(relative, biome);
    }

    /// <inheritdoc />
    public IEnumerable<Block> FindBlocks(BlockType type, IWorldIterator iterator, int? maxCount = null)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
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
    
    private void OnChunkBlockUpdate(IChunk chunk, int state, Position position)
    {
        var worldPosition = this.ToWorldPosition(chunk.Coordinates, position);
        this.OnBlockUpdated?.Invoke(this, new Block( // TODO: Hier jedes mal ein neuen block zu erstellen ist quatsch
            this.Data.Blocks.GetByState(state), state, worldPosition));
    }

    private int NonNegativeMod(int x, int m)
    {
        var v = x % m;
        if (v < 0)
            v += m;
        
        return v;
    }
}
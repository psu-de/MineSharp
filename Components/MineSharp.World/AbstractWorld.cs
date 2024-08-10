using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Events;
using MineSharp.Core.Geometry;
using MineSharp.Data;
using MineSharp.World.Chunks;
using MineSharp.World.Exceptions;
using MineSharp.World.Iterators;
using NLog;

namespace MineSharp.World;

/// <summary>
///     Base class for other implementations of the IWorld interface
/// </summary>
/// <param name="data"></param>
/// <param name="dimensionInfo"></param>
public abstract class AbstractWorld(MinecraftData data, DimensionInfo dimensionInfo) : IWorld, IAsyncWorld
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    ///     The MinecraftData instance used for this world
    /// </summary>
    public readonly MinecraftData Data = data;

    /// <summary>
    ///     The Dimension that this world represents
    /// </summary>
    public readonly DimensionInfo DimensionInfo = dimensionInfo;

    private readonly BlockInfo outOfMapBlock = data.Blocks.ByType(BlockType.Air)!;

    /// <summary>
    ///     A dictionary with all loaded chunks
    /// </summary>
    protected ConcurrentDictionary<ChunkCoordinates, IChunk> Chunks = new();

    /// <summary>
    ///     A dictionary that holds <see cref="TaskCompletionSource{TResult}" /> for chunks that are yet to be loaded
    ///     but are requested by some async method in this class.
    /// </summary>
    /// <remarks>
    ///     Implementation Note: This could also be done using the <see cref="OnChunkLoaded"/> event, but this way
    ///     has less overhead and is more robust. Since the user might remove the event listener.
    /// </remarks>
    protected ConcurrentDictionary<ChunkCoordinates, TaskCompletionSource<IChunk>> ChunkLoadAwaiters = new();

    /// <inheritdoc />
    public int MaxY => DimensionInfo.WorldMaxY;

    /// <inheritdoc />
    public int MinY => DimensionInfo.WorldMinY;


    /// <inheritdoc />
    public AsyncEvent<IWorld, IChunk> OnChunkLoaded { get; set; } = new();

    /// <inheritdoc />
    public AsyncEvent<IWorld, IChunk> OnChunkUnloaded { get; set; } = new();

    /// <inheritdoc />
    public AsyncEvent<IWorld, Block> OnBlockUpdated { get; set; } = new();

    /// <inheritdoc />
    [Pure]
    public ChunkCoordinates ToChunkCoordinates(Position position)
    {
        var chunkX = (int)Math.Floor((float)position.X / IChunk.Size);
        var chunkZ = (int)Math.Floor((float)position.Z / IChunk.Size);

        return new(chunkX, chunkZ);
    }

    /// <inheritdoc />
    [Pure]
    public Position ToChunkPosition(Position position)
    {
        return new(
            NonNegativeMod(position.X, IChunk.Size),
            position.Y,
            NonNegativeMod(position.Z, IChunk.Size));
    }

    /// <inheritdoc />
    [Pure]
    public Position ToWorldPosition(ChunkCoordinates coordinates, Position position)
    {
        var dx = coordinates.X * IChunk.Size;
        var dz = coordinates.Z * IChunk.Size;

        return new(position.X + dx, position.Y, position.Z + dz);
    }

    /// <inheritdoc />
    [Pure]
    public IChunk GetChunkAt(ChunkCoordinates coordinates)
    {
        if (!TryGetChunkAt(coordinates, out var chunk))
        {
            throw new ChunkNotLoadedException($"The chunk at {coordinates} is not loaded.");
        }

        return chunk;
    }

    /// <inheritdoc />
    [Pure]
    public bool TryGetChunkAt(ChunkCoordinates coordinates, [NotNullWhen(true)] out IChunk? chunk)
    {
        return Chunks.TryGetValue(coordinates, out chunk);
    }

    /// <inheritdoc />
    [Pure]
    public bool IsChunkLoaded(ChunkCoordinates coordinates)
    {
        return Chunks.ContainsKey(coordinates);
    }

    /// <inheritdoc />
    public void LoadChunk(IChunk chunk)
    {
        IChunk? oldChunk = null;
        // to be thread-safe, we need to use AddOrUpdate
        if (Chunks.AddOrUpdate(chunk.Coordinates, chunk, (key, oldValue) =>
        {
            oldChunk = oldValue;
            return chunk;
        }) != chunk)
        {
            throw new Exception($"Failed to update chunk at {chunk.Coordinates}. This should never happen.");
        }

        if (oldChunk != null)
        {
            oldChunk.OnBlockUpdated -= OnChunkBlockUpdate;
        }

        // we complete the chunk load awaiter before firing the events
        if (ChunkLoadAwaiters.TryRemove(chunk.Coordinates, out var tcs))
        {
            tcs.SetResult(chunk);
        }

        chunk.OnBlockUpdated += OnChunkBlockUpdate;
        OnChunkLoaded.Dispatch(this, chunk);
    }

    /// <inheritdoc />
    public void UnloadChunk(ChunkCoordinates coordinates)
    {
        if (!Chunks.TryRemove(coordinates, out var chunk))
        {
            Logger.Warn($"Trying to unload chunk which was not loaded {coordinates}.");
            return;
        }

        OnChunkUnloaded.Dispatch(this, chunk);
    }

    /// <inheritdoc />
    public abstract IChunk CreateChunk(ChunkCoordinates coordinates, BlockEntity[] entities);

    /// <inheritdoc />
    public abstract bool IsOutOfMap(Position position);

    /// <summary>
    /// Get a block that represents the out of map block at the given position.
    /// If the position is not out of map, this method returns null.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private Block? GetOutOfMapBlock(Position position)
    {
        if (IsOutOfMap(position))
        {
            return new(outOfMapBlock, outOfMapBlock.DefaultState, position);
        }
        return null;
    }

    /// <inheritdoc />
    public bool IsBlockLoaded(Position position, [NotNullWhen(true)] out IChunk? chunk)
    {
        return Chunks.TryGetValue(ToChunkCoordinates(position), out chunk);
    }

    private Block GetBlockFromChunk(Position position, IChunk chunk)
    {
        var relative = ToChunkPosition(position);
        var blockState = chunk.GetBlockAt(relative);
        return new Block(Data.Blocks.ByState(blockState)!, blockState, position);
    }

    /// <inheritdoc />
    public Block GetBlockAt(Position position)
    {
        var block = GetOutOfMapBlock(position);
        if (block != null)
        {
            return block;
        }

        if (!IsBlockLoaded(position, out var chunk))
        {
            throw new ChunkNotLoadedException($"Block at {position} is not loaded.");
        }

        return GetBlockFromChunk(position, chunk);
    }

    /// <inheritdoc />
    public void SetBlock(Block block)
    {
        if (IsOutOfMap(block.Position))
        {
            throw new InvalidOperationException("Cannot set block at out of map position.");
        }

        if (!IsBlockLoaded(block.Position, out var chunk))
        {
            throw new ChunkNotLoadedException($"Block at {block.Position} is not loaded.");
        }

        var relative = ToChunkPosition(block.Position);
        chunk.SetBlockAt(block.State, relative);
    }

    /// <inheritdoc />
    public Biome GetBiomeAt(Position position)
    {
        if (IsOutOfMap(position))
        {
            throw new InvalidOperationException("Cannot get biome at out of map position.");
        }

        if (!IsBlockLoaded(position, out var chunk))
        {
            throw new ChunkNotLoadedException($"Position {position} is not loaded.");
        }

        var relative = ToChunkPosition(position);
        return chunk.GetBiomeAt(relative);
    }

    /// <inheritdoc />
    public void SetBiomeAt(Position position, Biome biome)
    {
        if (!IsBlockLoaded(position, out var chunk))
        {
            throw new ChunkNotLoadedException($"Position {position} is not loaded.");
        }

        var relative = ToChunkPosition(position);
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
        var found = 0;
        foreach (var chunk in Chunks.Values)
        {
            foreach (var block in chunk.FindBlocks(type, maxCount - found))
            {
                block.Position = ToWorldPosition(chunk.Coordinates, block.Position);
                yield return block;

                found++;
                if (found >= maxCount)
                {
                    yield break;
                }
            }
        }
    }

    private void OnChunkBlockUpdate(IChunk chunk, int state, Position position)
    {
        var worldPosition = ToWorldPosition(chunk.Coordinates, position);
        OnBlockUpdated.Dispatch(this, new( // TODO: Hier jedes mal ein neuen block zu erstellen ist quatsch
                                     Data.Blocks.ByState(state)!, state, worldPosition));
    }

    private int NonNegativeMod(int x, int m)
    {
        var v = x % m;
        if (v < 0)
        {
            v += m;
        }

        return v;
    }

    #region IAsyncWorld

    private Task<IChunk> RegisterChunkAwaiter(ChunkCoordinates coordinates)
    {
        var tcs = ChunkLoadAwaiters.GetOrAdd(coordinates, _ => new TaskCompletionSource<IChunk>());
        // because there is a small chance that the chunk was loaded before we were able to add the awaiter
        // we need to check again if the chunk is loaded
        if (TryGetChunkAt(coordinates, out var chunk))
        {
            tcs.SetResult(chunk);
        }
        return tcs.Task;
    }

    /// <inheritdoc/>
    public Task<IChunk> GetChunkAtAsync(ChunkCoordinates coordinates)
    {
        if (Chunks.TryGetValue(coordinates, out var chunk))
        {
            return Task.FromResult(chunk);
        }

        // If the chunk is not loaded, we need to wait for it to be loaded
        return RegisterChunkAwaiter(coordinates);
    }

    private Task<IChunk> GetChunkForBlockPosAsync(Position position)
    {
        return GetChunkAtAsync(ToChunkCoordinates(position));
    }

    /// <inheritdoc/>
    public Task<Block> GetBlockAtAsync(Position position)
    {
        var block = GetOutOfMapBlock(position);
        if (block != null)
        {
            return Task.FromResult(block);
        }

        var blockTask = GetChunkForBlockPosAsync(position)
            .ContinueWith(chunkTask =>
            {
                var chunk = chunkTask.Result;
                return GetBlockFromChunk(position, chunk);
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        return blockTask;
    }

    /// <inheritdoc/>
    public Task SetBlockAsync(Block block)
    {
        if (IsOutOfMap(block.Position))
        {
            throw new InvalidOperationException("Cannot set block at out of map position.");
        }

        var blockTask = GetChunkForBlockPosAsync(block.Position)
            .ContinueWith(chunkTask =>
            {
                var chunk = chunkTask.Result;
                var relative = ToChunkPosition(block.Position);
                chunk.SetBlockAt(block.State, relative);
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        return blockTask;
    }

    /// <inheritdoc/>
    public Task<Biome> GetBiomeAtAsync(Position position)
    {
        if (IsOutOfMap(position))
        {
            throw new InvalidOperationException("Cannot get biome at out of map position.");
        }

        var biomeTask = GetChunkForBlockPosAsync(position)
            .ContinueWith(chunkTask =>
            {
                var chunk = chunkTask.Result;
                var relative = ToChunkPosition(position);
                return chunk.GetBiomeAt(relative);
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        return biomeTask;
    }

    /// <inheritdoc/>
    public Task SetBiomeAtAsync(Position position, Biome biome)
    {
        if (IsOutOfMap(position))
        {
            throw new InvalidOperationException("Cannot set biome at out of map position.");
        }

        var biomeTask = GetChunkForBlockPosAsync(position)
            .ContinueWith(chunkTask =>
            {
                var chunk = chunkTask.Result;
                var relative = ToChunkPosition(position);
                chunk.SetBiomeAt(relative, biome);
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        return biomeTask;
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<Block> FindBlocksAsync(BlockType type, IWorldIterator iterator, int? maxCount = null)
    {
        // can be implemented once FindBlocks is implemented
        throw new NotImplementedException();
    }

    #endregion
}

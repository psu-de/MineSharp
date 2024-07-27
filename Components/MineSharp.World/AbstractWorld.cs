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
public abstract class AbstractWorld(MinecraftData data) : IWorld
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    ///     The MinecraftData instance used for this world
    /// </summary>
    public readonly MinecraftData Data = data;

    private readonly BlockInfo outOfMapBlock = data.Blocks.ByType(BlockType.Air)!;

    /// <summary>
    ///     A dictionary with all loaded chunks
    /// </summary>
    protected ConcurrentDictionary<ChunkCoordinates, IChunk> Chunks = new();

    /// <inheritdoc />
    public abstract int MaxY { get; }

    /// <inheritdoc />
    public abstract int MinY { get; }


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
    public IChunk GetChunkAt(ChunkCoordinates coordinates)
    {
        if (!Chunks.TryGetValue(coordinates, out var chunk))
        {
            throw new ChunkNotLoadedException($"The chunk at {coordinates} is not loaded.");
        }

        return chunk;
    }

    /// <inheritdoc />
    [Pure]
    public bool IsChunkLoaded(ChunkCoordinates coordinates)
    {
        return IsChunkLoaded(coordinates, out _);
    }

    /// <inheritdoc />
    public void LoadChunk(IChunk chunk)
    {
        if (IsChunkLoaded(chunk.Coordinates, out var oldChunk))
        {
            oldChunk.OnBlockUpdated -= OnChunkBlockUpdate;
            Chunks[chunk.Coordinates] = chunk;
        }
        else
        {
            Chunks.TryAdd(chunk.Coordinates, chunk);
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

    /// <inheritdoc />
    public bool IsBlockLoaded(Position position, [NotNullWhen(true)] out IChunk? chunk)
    {
        return Chunks.TryGetValue(ToChunkCoordinates(position), out chunk);
    }

    /// <inheritdoc />
    public Block GetBlockAt(Position position)
    {
        if (IsOutOfMap(position))
        {
            return new(outOfMapBlock, outOfMapBlock.DefaultState, position);
        }

        if (!IsBlockLoaded(position, out var chunk))
        {
            throw new ChunkNotLoadedException($"Block at {position} is not loaded.");
        }

        var relative = ToChunkPosition(position);
        var blockState = chunk.GetBlockAt(relative);
        var block = new Block(
            Data.Blocks.ByState(blockState)!,
            blockState,
            position);
        return block;
    }

    /// <inheritdoc />
    public void SetBlock(Block block)
    {
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

    /// <summary>
    ///     Mutate <paramref name="position" /> to a world position
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="position"></param>
    protected void MutateToWorldPosition(ChunkCoordinates coordinates, MutablePosition position)
    {
        var dx = coordinates.X * IChunk.Size;
        var dz = coordinates.Z * IChunk.Size;
        position.Set(position.X + dx, position.Y, position.Z + dz);
    }

    /// <inheritdoc />
    [Pure]
    protected bool IsChunkLoaded(ChunkCoordinates coordinates, [NotNullWhen(true)] out IChunk? chunk)
    {
        return Chunks.TryGetValue(coordinates, out chunk);
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
}

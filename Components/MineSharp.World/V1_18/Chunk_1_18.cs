using MineSharp.Core.Common;
using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Blocks;
using MineSharp.Data;
using MineSharp.World.Chunks;
using MineSharp.World.Exceptions;

namespace MineSharp.World.V1_18;

/// <summary>
/// Chunk implementation for >= 1.18
/// </summary>
public sealed class Chunk_1_18 : IChunk
{
    private const int SECTION_COUNT = World_1_18.WORLD_HEIGHT / ChunkSection_1_18.SECTION_SIZE;

    /// <inheritdoc />
    public ChunkCoordinates Coordinates { get; }

    /// <inheritdoc />
    public event Events.ChunkBlockEvent? OnBlockUpdated;

    private readonly MinecraftData                     _data;
    private readonly IChunkSection[]                   _sections;
    private readonly Dictionary<Position, BlockEntity> _blockEntities;

    /// <summary>
    /// Create a new instance
    /// </summary>
    /// <param name="data"></param>
    /// <param name="coordinates"></param>
    /// <param name="blockEntities"></param>
    public Chunk_1_18(MinecraftData data, ChunkCoordinates coordinates, BlockEntity[] blockEntities)
    {
        this.Coordinates    = coordinates;
        this._data          = data;
        this._blockEntities = new Dictionary<Position, BlockEntity>();
        this._sections      = new IChunkSection[SECTION_COUNT];

        foreach (var entity in blockEntities)
        {
            this._blockEntities.Add(new Position(entity.X, entity.Y, entity.Z), entity);
        }
    }

    /// <inheritdoc />
    public void LoadData(byte[] data)
    {
        var buffer = new PacketBuffer(data, this._data.Version.Protocol >= 764);
        for (int i = 0; i < SECTION_COUNT; i++)
        {
            this._sections[i] = ChunkSection_1_18.FromStream(this._data, buffer);
        }
    }

    /// <inheritdoc />
    public BlockEntity? GetBlockEntity(Position position)
    {
        this._blockEntities.TryGetValue(position, out var entity);
        return entity;
    }

    /// <inheritdoc />
    public int GetBlockAt(Position position)
    {
        (var y, var section) = GetChunkSectionAndNewYFromPosition(position);
        var block = section.GetBlockAt(new Position(position.X, y, position.Z));

        return block;
    }

    /// <inheritdoc />
    public void SetBlockAt(int state, Position position)
    {
        (var y, var section) = GetChunkSectionAndNewYFromPosition(position);
        section.SetBlockAt(state, new Position(position.X, y, position.Z));

        this.OnBlockUpdated?.Invoke(this, state, position);
    }

    /// <inheritdoc />
    public Biome GetBiomeAt(Position position)
    {
        (var y, var section) = GetChunkSectionAndNewYFromPosition(position);
        return section.GetBiomeAt(new Position(position.X, y, position.Z));
    }

    /// <inheritdoc />
    public void SetBiomeAt(Position position, Biome biome)
    {
        (var y, var section) = GetChunkSectionAndNewYFromPosition(position);
        section.SetBiomeAt(new Position(position.X, y, position.Z), biome);
    }

    /// <inheritdoc />
    public IEnumerable<Block> FindBlocks(BlockType type, int? maxCount = null)
    {
        int found = 0;
        for (int i = 0; i < this._sections.Length; i++)
        {
            var left    = maxCount - found;
            var section = this._sections[i];

            if (section == null)
                continue;

            foreach (var block in section.FindBlocks(type, left))
            {
                block.Position = new Position(
                    block.Position.X,
                    FromChunkSectionY(block.Position.Y, i),
                    block.Position.Z);
                yield return block;

                if (++found >= maxCount)
                    yield break;
            }
        }
    }

    private (int y, IChunkSection section) GetChunkSectionAndNewYFromPosition(Position position)
    {
        var sectionIndex = this.GetSectionIndex(position.Y);
        if (sectionIndex >= this._sections.Length)
            throw new OutOfWorldException($"The Y coordinate {position.Y} is out of the world.");

        var y = this.ToChunkSectionY(position.Y);

        return (y, this._sections[sectionIndex]);
    }

    private int ToChunkSectionY(int y)
    {
        int v = y % ChunkSection_1_18.SECTION_SIZE;
        if (v < 0)
            v += ChunkSection_1_18.SECTION_SIZE;
        return v;
    }

    private int FromChunkSectionY(int y, int sectionIndex)
    {
        const int negativeSectionCount = -World_1_18.MIN_Y / ChunkSection_1_18.SECTION_SIZE;
        return (sectionIndex - negativeSectionCount) * ChunkSection_1_18.SECTION_SIZE + y;
    }

    private int GetSectionIndex(int y) => (y - World_1_18.MIN_Y) / ChunkSection_1_18.SECTION_SIZE;
}

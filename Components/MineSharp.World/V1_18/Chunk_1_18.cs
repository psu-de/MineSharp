using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Events;
using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.World.Chunks;
using MineSharp.World.Exceptions;

namespace MineSharp.World.V1_18;

/// <summary>
///     Chunk implementation for >= 1.18
/// </summary>
public sealed class Chunk118 : IChunk
{
    private readonly Dictionary<Position, BlockEntity> blockEntities;

    private readonly MinecraftData data;
    private readonly DimensionInfo dimensionInfo;
    private readonly IChunkSection[] sections;

    private int CalculateSectionCount() => dimensionInfo.GetWorldHeight() / ChunkSection118.SectionSize;

    /// <summary>
    ///     Create a new instance
    /// </summary>
    /// <param name="data"></param>
    /// <param name="dimensionInfo"></param>
    /// <param name="coordinates"></param>
    /// <param name="blockEntities"></param>
    public Chunk118(MinecraftData data, DimensionInfo dimensionInfo, ChunkCoordinates coordinates, BlockEntity[] blockEntities)
    {
        Coordinates = coordinates;
        this.data = data;
        this.dimensionInfo = dimensionInfo;
        this.blockEntities = new();
        sections = new IChunkSection[CalculateSectionCount()];

        foreach (var entity in blockEntities)
        {
            this.blockEntities.Add(new(entity.X, entity.Y, entity.Z), entity);
        }
    }

    /// <inheritdoc />
    public ChunkCoordinates Coordinates { get; }

    /// <inheritdoc />
    public AsyncEvent<IChunk, int, Position> OnBlockUpdated { get; set; } = new();

    /// <inheritdoc />
    public void LoadData(byte[] buf)
    {
        var buffer = new PacketBuffer(buf, data.Version.Protocol);
        for (var i = 0; i < sections.Length; i++)
        {
            sections[i] = ChunkSection118.FromStream(data, buffer);
        }
    }

    /// <inheritdoc />
    public BlockEntity? GetBlockEntity(Position position)
    {
        blockEntities.TryGetValue(position, out var entity);
        return entity;
    }

    /// <inheritdoc />
    public int GetBlockAt(Position position)
    {
        (var y, var section) = GetChunkSectionAndNewYFromPosition(position);
        var block = section.GetBlockAt(new(position.X, y, position.Z));

        return block;
    }

    /// <inheritdoc />
    public void SetBlockAt(int state, Position position)
    {
        (var y, var section) = GetChunkSectionAndNewYFromPosition(position);
        section.SetBlockAt(state, new(position.X, y, position.Z));

        OnBlockUpdated.Dispatch(this, state, position);
    }

    /// <inheritdoc />
    public Biome GetBiomeAt(Position position)
    {
        (var y, var section) = GetChunkSectionAndNewYFromPosition(position);
        return section.GetBiomeAt(new(position.X, y, position.Z));
    }

    /// <inheritdoc />
    public void SetBiomeAt(Position position, Biome biome)
    {
        (var y, var section) = GetChunkSectionAndNewYFromPosition(position);
        section.SetBiomeAt(new(position.X, y, position.Z), biome);
    }

    /// <inheritdoc />
    public IEnumerable<Block> FindBlocks(BlockType type, int? maxCount = null)
    {
        var found = 0;
        for (var i = 0; i < sections.Length; i++)
        {
            var left = maxCount - found;
            var section = sections[i];

            foreach (var block in section.FindBlocks(type, left))
            {
                block.Position = new(
                    block.Position.X,
                    FromChunkSectionY(block.Position.Y, i, dimensionInfo.WorldMinY),
                    block.Position.Z);
                yield return block;

                if (++found >= maxCount)
                {
                    yield break;
                }
            }
        }
    }

    private (int y, IChunkSection section) GetChunkSectionAndNewYFromPosition(Position position)
    {
        var sectionIndex = GetSectionIndex(position.Y, dimensionInfo.WorldMinY);
        if (sectionIndex >= sections.Length)
        {
            throw new OutOfWorldException($"The Y coordinate {position.Y} is out of the world.");
        }

        var y = ToChunkSectionY(position.Y);

        return (y, sections[sectionIndex]);
    }

    private int ToChunkSectionY(int y)
    {
        var v = y % ChunkSection118.SectionSize;
        if (v < 0)
        {
            v += ChunkSection118.SectionSize;
        }

        return v;
    }

    private static int FromChunkSectionY(int y, int sectionIndex, int worldMinY)
    {
        int negative_section_count = -worldMinY / ChunkSection118.SectionSize;
        return ((sectionIndex - negative_section_count) * ChunkSection118.SectionSize) + y;
    }

    private static int GetSectionIndex(int y, int worldMinY)
    {
        return (y - worldMinY) / ChunkSection118.SectionSize;
    }
}

using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.World.Chunks;
using MineSharp.World.Containers;

namespace MineSharp.World.V1_18;

internal class ChunkSection118 : IChunkSection
{
    public const int SectionSize = 16;
    private readonly BiomeContainer biomeContainer;
    private readonly BlockContainer blockContainer;

    private readonly MinecraftData data;

    public ChunkSection118(MinecraftData data, short blockCount, BlockContainer blocks, BiomeContainer biomes)
    {
        this.data = data;
        solidBlockCount = blockCount;
        blockContainer = blocks;
        biomeContainer = biomes;
    }

    private int solidBlockCount;
    public short SolidBlockCount => (short)solidBlockCount;

    public int GetBlockAt(Position position)
    {
        return blockContainer.GetAt(GetBlockIndex(position));
    }

    public void SetBlockAt(int state, Position position)
    {
        var index = GetBlockIndex(position);
        var old = data.Blocks.ByState(
            blockContainer.GetAt(index))!;

        var wasSolid = old.IsSolid();
        var isSolid = data.Blocks.ByState(state)!.IsSolid();

        if (wasSolid != isSolid)
        {
            if (isSolid)
            {
                Interlocked.Increment(ref solidBlockCount);
            }
            else
            {
                Interlocked.Decrement(ref solidBlockCount);
            }
        }

        blockContainer.SetAt(index, state);
    }

    public Biome GetBiomeAt(Position position)
    {
        var index = GetBiomeIndex(position);
        var state = biomeContainer.GetAt(index);
        return new(data.Biomes.ById(state)!);
    }

    public void SetBiomeAt(Position position, Biome biome)
    {
        var index = GetBiomeIndex(position);
        biomeContainer.SetAt(index, biome.Info.Id);
    }

    public IEnumerable<Block> FindBlocks(BlockType type, int? maxCount = null)
    {
        var info = data.Blocks.ByType(type)!;
        if (!blockContainer.Palette.ContainsState(info.MinState, info.MaxState) || maxCount == 0)
        {
            yield break;
        }

        var found = 0;
        for (var y = 0; y < SectionSize; y++)
        {
            for (var z = 0; z < SectionSize; z++)
            {
                for (var x = 0; x < SectionSize; x++)
                {
                    var index = GetBlockIndex(x, y, z);
                    var value = blockContainer.GetAt(index);
                    if (value < info.MinState || value > info.MaxState)
                    {
                        continue;
                    }

                    yield return new(info, value, new(x, y, z));

                    found++;

                    if (found >= maxCount)
                    {
                        yield break;
                    }
                }
            }
        }
    }


    private int GetBiomeIndex(Position position)
    {
        return ((position.Y >> 2) << 2) | ((position.Z >> 2) << 2) | (position.X >> 2);
    }

    private int GetBlockIndex(Position position)
    {
        return GetBlockIndex(position.X, position.Y, position.Z);
    }

    private int GetBlockIndex(int x, int y, int z)
    {
        return (y << 8) | (z << 4) | x;
    }

    internal static ChunkSection118 FromStream(MinecraftData data, PacketBuffer buffer)
    {
        var solidBlockCount = buffer.ReadShort();
        var blockContainer = BlockContainer.FromStream(data, buffer);
        var biomeContainer = BiomeContainer.FromStream(data, buffer);

        return new(data, solidBlockCount, blockContainer, biomeContainer);
    }
}

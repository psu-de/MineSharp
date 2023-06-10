using MineSharp.Core.Common;
using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Extensions;
using MineSharp.Data;
using MineSharp.World.Chunks;
using MineSharp.World.Containers;

namespace MineSharp.World.V1_18;

internal class ChunkSection_1_18 : IChunkSection
{
    public const int SECTION_SIZE = 16;
    
    private readonly MinecraftData _data;
    private readonly BiomeContainer _biomeContainer;
    private readonly BlockContainer _blockContainer;

    public short SolidBlockCount { get; set; }

    public ChunkSection_1_18(MinecraftData data, short blockCount, BlockContainer blocks, BiomeContainer biomes)
    {
        this._data = data;
        this.SolidBlockCount = blockCount;
        this._blockContainer = blocks;
        this._biomeContainer = biomes;
    }

    public int GetBlockAt(Position position)
    {
        return this._blockContainer.GetAt(this.GetBlockIndex(position));
    }
    
    public void SetBlockAt(int state, Position position)
    {
        var index = GetBlockIndex(position);
        var old = this._data.Blocks.GetByState( 
            this._blockContainer.GetAt(index));

        bool wasSolid = this._data.Blocks.IsSolid(old.Id);
        bool isSolid = this._data.Blocks.IsSolid(
            this._data.Blocks.GetByState(state).Id);

        if (wasSolid != isSolid)
        {
            if (isSolid)
                this.SolidBlockCount++;
            else
                this.SolidBlockCount--;
        }

        this._blockContainer.SetAt(index, state);
    }

    public Biome GetBiomeAt(Position position)
    {
        var index = GetBiomeIndex(position);
        var state = this._biomeContainer.GetAt(index);
        return new Biome(this._data.Biomes.GetById(state));
    }
    
    public void SetBiomeAt(Position position, Biome biome)
    {
        var index = GetBiomeIndex(position);
        this._biomeContainer.SetAt(index, biome.Info.Id);
    }

    public IEnumerable<Block> FindBlocks(int blockId, int? maxCount = null)
    {
        var info = this._data.Blocks.GetById(blockId);
        if (!this._blockContainer.Palette.ContainsState(info.MinStateId, info.MaxStateId) || maxCount == 0)
        {
            yield break;
        }

        int found = 0;
        for (var y = 0; y < SECTION_SIZE; y++)
        {
            for (var z = 0; z < SECTION_SIZE; z++)
            {
                for (var x = 0; x < SECTION_SIZE; x++)
                {
                    var index = GetBlockIndex(x, y, z);
                    var value = this._blockContainer.GetAt(index);
                    if (value < info.MinStateId || value > info.MaxStateId)
                        continue;
                    
                    yield return new Block(info, value, new Position(x, y, z));
                    found++;

                    if (found >= maxCount)
                        yield break;
                }
            }
        }
    }

    
    private int GetBiomeIndex(Position position)
    {
        return position.Y >> 2 << 2 | position.Z >> 2 << 2 | position.X >> 2;
    }

    private int GetBlockIndex(Position position)
        => GetBlockIndex(position.X, position.Y, position.Z);

    private int GetBlockIndex(int x, int y, int z)
        => y << 8 | z << 4 | x;

    internal static ChunkSection_1_18 FromStream(MinecraftData data, Stream buffer)
    {
        short solidBlockCount = buffer.ReadShort();
        var blockContainer = BlockContainer.FromStream(data, buffer);
        var biomeContainer = BiomeContainer.FromStream(data, buffer);

        return new ChunkSection_1_18(data, solidBlockCount, blockContainer, biomeContainer);
    }
}

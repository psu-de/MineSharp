using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Geometry;
using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;

namespace MineSharp.Data.BlockCollisionShapes;

internal class BlockCollisionShapeData(IDataProvider<BlockCollisionShapeDataBlob> provider)
    : IndexedData<BlockCollisionShapeDataBlob>(provider), IBlockCollisionShapeData
{
    private Dictionary<int, Aabb[]> indexToShape = new();
    private Dictionary<BlockType, int[]> typeToIndices = new();

    public int[] GetShapeIndices(BlockType type)
    {
        if (!Loaded)
        {
            Load();
        }

        return typeToIndices[type];
    }

    public Aabb[] GetShapes(int shapeIndex)
    {
        if (!Loaded)
        {
            Load();
        }

        return indexToShape[shapeIndex];
    }

    protected override void InitializeData(BlockCollisionShapeDataBlob data)
    {
        typeToIndices = data.BlockToIndicesMap;
        indexToShape = data.IndexToShapeMap
                           .ToDictionary(
                                x => x.Key,
                                x => x.Value.Select(y => new Aabb(y[0], y[1], y[2], y[3], y[4], y[5])).ToArray());
    }
}

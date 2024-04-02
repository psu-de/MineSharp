using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Geometry;
using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;

namespace MineSharp.Data.BlockCollisionShapes;

internal class BlockCollisionShapeData(IDataProvider<BlockCollisionShapeDataBlob> provider)
    : IndexedData<BlockCollisionShapeDataBlob>(provider), IBlockCollisionShapeData
{
    private Dictionary<BlockType, int[]> typeToIndices = new();
    private Dictionary<int, AABB[]>   indexToShape  = new();

    protected override void InitializeData(BlockCollisionShapeDataBlob data)
    {
        this.typeToIndices = data.BlockToIndicesMap;
        this.indexToShape  = data.IndexToShapeMap
                                 .ToDictionary(
                                      x => x.Key, 
                                      x => x.Value.Select(y => new AABB(y[0], y[1], y[2], y[3], y[4], y[5])).ToArray());
    }

    public int[] GetShapeIndices(BlockType type)
    {
        if (!this.Loaded)
            this.Load();

        return this.typeToIndices[type];
    }

    public AABB[] GetShapes(int shapeIndex)
    {
        if (!this.Loaded)
            this.Load();
        
        return indexToShape[shapeIndex];
    }
}

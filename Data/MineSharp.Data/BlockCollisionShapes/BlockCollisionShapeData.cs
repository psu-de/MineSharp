using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;

namespace MineSharp.Data.BlockCollisionShapes;

internal class BlockCollisionShapeData(IDataProvider<BlockCollisionShapeDataBlob> provider)
    : IndexedData<BlockCollisionShapeDataBlob>(provider), IBlockCollisionShapeData
{
    private Dictionary<BlockType, int[]> typeToIndices = new();
    private Dictionary<int, float[][]>   indexToShape  = new();

    protected override void InitializeData(BlockCollisionShapeDataBlob data)
    {
        this.typeToIndices = data.BlockToIndicesMap;
        this.indexToShape  = data.IndexToShapeMap;
    }

    public int[] GetShapeIndices(BlockType type)
    {
        if (!this.Loaded)
            this.Load();

        return this.typeToIndices[type];
    }

    public AABB[] GetShapes(int shapeIndex)
        => indexToShape[shapeIndex] // TODO: Use pooled AABB's
          .Select(x => new AABB(x[0], x[1], x[2], x[3], x[4], x[5]))
          .ToArray();
}

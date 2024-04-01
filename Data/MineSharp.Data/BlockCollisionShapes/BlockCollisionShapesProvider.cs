using MineSharp.Core.Common.Blocks;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;
using Newtonsoft.Json.Linq;

namespace MineSharp.Data.BlockCollisionShapes;

internal class BlockCollisionShapesProvider(JToken token) : IDataProvider<BlockCollisionShapeDataBlob>
{
    private static readonly EnumNameLookup<BlockType> BlockTypeLookup = new();

    public BlockCollisionShapeDataBlob GetData()
    {
        var blocks = (JObject)token.SelectToken("blocks")!;
        var shapes = (JObject)token.SelectToken("shapes")!;

        var blockDict = blocks.Properties()
                              .ToDictionary(
                                   x => BlockTypeLookup.FromName(NameUtils.GetBlockName(x.Name)),
                                   x => ToIntArray(x.Value));

        var shapesDict = shapes.Properties()
                               .ToDictionary(
                                    x => Convert.ToInt32(x.Name),
                                    x => ToFloatArray(x.Value));

        return new BlockCollisionShapeDataBlob(
            blockDict,
            shapesDict);
    }

    private int[] ToIntArray(JToken value)
    {
        if (value.Type == JTokenType.Integer)
            return [(int)value];

        return value.ToObject<int[]>()!;
    }

    private float[][] ToFloatArray(JToken value)
    {
        return value.ToObject<float[][]>()!;
    }
}

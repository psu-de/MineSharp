using Humanizer;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Biomes;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;
using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Biomes;

internal class BiomeProvider : IDataProvider<BiomeInfo[]>
{
    private static readonly EnumNameLookup<BiomeType>     BiomeTypeLookup = new();
    private static readonly EnumNameLookup<BiomeCategory> CategoryLookup  = new();
    private static readonly EnumNameLookup<Dimension>     DimensionLookup = new();

    private readonly JArray token;

    public BiomeProvider(JToken token)
    {
        if (token.Type != JTokenType.Array)
        {
            throw new InvalidOperationException("Expected the token to be an array");
        }

        this.token = (token as JArray)!;
    }

    public BiomeInfo[] GetData()
    {
        var length = token.Count;
        var data   = new BiomeInfo[length];

        for (int i = 0; i < length; i++)
        {
            data[i] = FromToken(token[i]!);
        }

        return data;
    }

    private static BiomeInfo FromToken(JToken dataToken)
    {
        var id            = (int)dataToken.SelectToken("id")!;
        var name          = (string)dataToken.SelectToken("name")!;
        var displayName   = (string)dataToken.SelectToken("displayToken")!;
        var category      = (string)dataToken.SelectToken("category")!;
        var temperature   = (float)dataToken.SelectToken("temperature")!;
        var precipitation = (string)dataToken.SelectToken("precipitation")! != "none";
        var dimension     = (string)dataToken.SelectToken("dimension")!;
        var color         = (int)dataToken.SelectToken("color")!;

        return new BiomeInfo(
            id,
            BiomeTypeLookup.FromName(NameUtils.GetBiomeName(name)),
            name,
            displayName,
            CategoryLookup.FromName(NameUtils.GetBiomeCategory(category)),
            temperature,
            precipitation,
            DimensionLookup.FromName(NameUtils.GetDimensionName(dimension)),
            color
        );
    }
}

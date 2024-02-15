using Humanizer;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Biomes;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;
using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Biomes;

internal class BiomeProvider(JToken token) : IDataProvider<BiomeInfo[]>
{
    private EnumNameLookup<BiomeType> typeLookup = new();
    private EnumNameLookup<BiomeCategory> categoryLookup = new();
    private EnumNameLookup<Dimension> dimensionLookup = new();

    public BiomeInfo[] GetData()
    {
        if (token.Type != JTokenType.Array)
        {
            throw new InvalidOperationException("Expected data token to be an array");
        }

        var length = token.Count();
        var data = new BiomeInfo[length];

        for (int i = 0; i < length; i++)
        {
            data[i] = TokenToBiome(token[i]!);
        }

        return data;
    }

    private BiomeInfo TokenToBiome(JToken dataToken)
    {
        var id = (int)dataToken.SelectToken("id")!;
        var name = (string)dataToken.SelectToken("name")!;
        var displayName = (string)dataToken.SelectToken("displayToken")!;
        var category = (string)dataToken.SelectToken("category")!;
        var temperature = (float)dataToken.SelectToken("temperature")!;
        var precipitation = (string)dataToken.SelectToken("precipitation")! != "none";
        var dimension = (string)dataToken.SelectToken("dimension")!;
        var color = (int)dataToken.SelectToken("color")!;

        return new BiomeInfo(
            id,
            typeLookup.FromName(NameUtils.GetBiomeName(name)),
            name,
            displayName,
            categoryLookup.FromName(category.Pascalize()),
            temperature,
            precipitation,
            dimensionLookup.FromName(dimension),
            color
        );
    }
}
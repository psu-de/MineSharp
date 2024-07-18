using MineSharp.Core.Common.Effects;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;
using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Effects;

internal class EffectProvider : IDataProvider<EffectInfo[]>
{
    private static readonly EnumNameLookup<EffectType> EffectTypeLookup = new();

    private readonly JArray token;

    public EffectProvider(JToken token)
    {
        if (token.Type != JTokenType.Array)
        {
            throw new ArgumentException($"expected {JTokenType.Array}, got {token.Type}");
        }

        this.token = (JArray)token;
    }

    public EffectInfo[] GetData()
    {
        var data = new EffectInfo[token.Count];

        for (var i = 0; i < token.Count; i++)
        {
            data[i] = FromToken(token[i]);
        }

        return data;
    }

    private static EffectInfo FromToken(JToken token)
    {
        var id = (int)token.SelectToken("id")!;
        var name = (string)token.SelectToken("name")!;
        var displayName = (string)token.SelectToken("displayName")!;
        var isGood = (string)token.SelectToken("type")! == "good";

        return new(
            id,
            EffectTypeLookup.FromName(NameUtils.GetEffectName(name)),
            name,
            displayName,
            isGood);
    }
}

using System.Collections.Frozen;
using MineSharp.Core.Common;
using MineSharp.Data.Framework.Providers;
using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Particles;

internal class ParticleDataProvider : IDataProvider<IReadOnlyDictionary<Identifier, int>>
{
    private readonly JArray token;
    
    public ParticleDataProvider(JToken token)
    {
        if (token.Type != JTokenType.Array)
        {
            throw new ArgumentException("Expected the token to be an array");
        }

        this.token = (token as JArray)!;
    }
    
    public IReadOnlyDictionary<Identifier, int> GetData()
    {
        return token
              .Select(FromToken)
              .ToFrozenDictionary(x => x.Key, x => x.Value);
    }

    private static KeyValuePair<Identifier, int> FromToken(JToken token)
    {
        var name = (string)token["name"]!;
        var id = (int)token["id"]!;

        return new (Identifier.Parse(name), id);
    }
}

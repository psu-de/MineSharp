using MineSharp.Core.Registries;
using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Utils;

internal static class RegistryHelper
{
    public delegate Task<JToken> FetchDelegate();
    public delegate T OrderBy<out T>(JToken token);
    public delegate TInfo Parser<out TInfo>(JToken token);
    
    public static async Task<TRegistry> LoadRegistry<TRegistry, TInfo, TEnum, T>(FetchDelegate fetch, Parser<TInfo> parser, OrderBy<T>? orderBy = null)
        where TRegistry : Registry<TInfo, TEnum>, new()
        where TInfo : IRegistryObject<TEnum>
        where TEnum : struct, Enum
    {
        var token = await fetch();
        
        if (token.Type != JTokenType.Array)
        {
            throw new InvalidOperationException($"Expected an array, got {token.Type}.");
        }

        var registry = new TRegistry();
        IEnumerable<JToken> values = token;

        if (orderBy != null)
        {
            values = values.OrderBy(x => orderBy(x));
        }
        
        foreach (var element in values)
        {
            registry.Register(parser(element));
        }

        return registry;
    }

    public static Task<TRegistry> LoadRegistry<TRegistry, TInfo, TEnum>(
        FetchDelegate fetch, Parser<TInfo> parser)
        where TRegistry : Registry<TInfo, TEnum>, new()
        where TInfo : IRegistryObject<TEnum>
        where TEnum : struct, Enum
        => LoadRegistry<TRegistry, TInfo, TEnum, object>(fetch, parser, null);
}

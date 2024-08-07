using System.Collections.Frozen;
using MineSharp.Core.Common.Protocol;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;
using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Protocol;

internal class ProtocolProvider : IDataProvider<ProtocolDataBlob>
{
    private static readonly EnumNameLookup<GameState> StateLookup = new();
    private static readonly EnumNameLookup<PacketType> TypeLookup = new();

    private readonly JObject token;

    public ProtocolProvider(JToken token)
    {
        if (token.Type != JTokenType.Object)
        {
            throw new ArgumentException("Expected token to be an object");
        }

        this.token = (JObject)token;
    }

    public ProtocolDataBlob GetData()
    {
        var byFlow = new Dictionary<PacketFlow, Dictionary<GameState, FrozenDictionary<int, PacketType>>>
        {
            { PacketFlow.Clientbound, new Dictionary<GameState, FrozenDictionary<int, PacketType>>() },
            { PacketFlow.Serverbound, new Dictionary<GameState, FrozenDictionary<int, PacketType>>() }
        };

        foreach (var ns in token.Properties())
        {
            if (ns.Name == "types")
            {
                continue;
            }

            var state = StateLookup.FromName(NameUtils.GetGameState(ns.Name));

            var cbPackets = CollectPackets(ns.Name, "toClient");
            var sbPackets = CollectPackets(ns.Name, "toServer");

            byFlow[PacketFlow.Clientbound].Add(state, cbPackets);
            byFlow[PacketFlow.Serverbound].Add(state, sbPackets);
        }

        var frozenDict = byFlow.ToFrozenDictionary(x => x.Key, y => y.Value.ToFrozenDictionary());
        return new(frozenDict);
    }

    private FrozenDictionary<int, PacketType> CollectPackets(string ns, string direction)
    {
        var obj = (JObject)token.SelectToken($"{ns}.{direction}.types.packet[1][0].type[1].mappings")!;

        return obj.Properties()
                  .ToFrozenDictionary(
                       x => Convert.ToInt32(x.Name, 16),
                       x => TypeLookup.FromName(
                           NameUtils.GetPacketName(
                               (string)x.Value!,
                               direction,
                               ns))
                   );
    }
}

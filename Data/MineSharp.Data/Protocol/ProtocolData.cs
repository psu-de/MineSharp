using System.Collections.Frozen;
using MineSharp.Core.Common.Protocol;
using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;
using NLog;

namespace MineSharp.Data.Protocol;

internal class ProtocolData(IDataProvider<ProtocolDataBlob> provider)
    : IndexedData<ProtocolDataBlob>(provider), IProtocolData
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private FrozenDictionary<PacketFlow, FrozenDictionary<GameState, FrozenDictionary<int, PacketType>>> idToType = FrozenDictionary<PacketFlow, FrozenDictionary<GameState, FrozenDictionary<int, PacketType>>>.Empty;
    private FrozenDictionary<PacketType, int> typeToId = FrozenDictionary<PacketType, int>.Empty;

    public int GetPacketId(PacketType type)
    {
        if (!Loaded)
        {
            Load();
        }

        return typeToId[type];
    }

    public PacketType GetPacketType(PacketFlow flow, GameState state, int id)
    {
        if (!Loaded)
        {
            Load();
        }

        try
        {
            return idToType[flow][state][id];
        }
        catch (Exception)
        {
            Logger.Error("Failed to get PacketType for: flow = {Flow}, state = {State}, id = {Id}", flow, state, id);
            throw;
        }
    }

    protected override void InitializeData(ProtocolDataBlob data)
    {
        idToType = data.IdToTypeMap;

        typeToId = idToType.Values
                           .SelectMany(x => x.Values)
                           .SelectMany(x => x.ToArray())
                           .ToFrozenDictionary(x => x.Value, x => x.Key);
    }
}

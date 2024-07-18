using MineSharp.Core.Common.Protocol;
using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;

namespace MineSharp.Data.Protocol;

internal class ProtocolData(IDataProvider<ProtocolDataBlob> provider)
    : IndexedData<ProtocolDataBlob>(provider), IProtocolData
{
    private Dictionary<PacketFlow, Dictionary<GameState, Dictionary<int, PacketType>>> idToType = new();
    private Dictionary<PacketType, int> typeToId = new();

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

        return idToType[flow][state][id];
    }

    protected override void InitializeData(ProtocolDataBlob data)
    {
        idToType = data.IdToTypeMap;

        typeToId = idToType.Values
                           .SelectMany(x => x.Values)
                           .SelectMany(x => x.ToArray())
                           .ToDictionary(x => x.Value, x => x.Key);
    }
}

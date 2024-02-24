using MineSharp.Core.Common.Protocol;
using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;

namespace MineSharp.Data.Protocol;

internal class ProtocolData(IDataProvider<ProtocolDataBlob> provider) : IndexedData<ProtocolDataBlob>(provider), IProtocolData
{
    private Dictionary<PacketType, int>                                                typeToId = new();
    private Dictionary<PacketFlow, Dictionary<GameState, Dictionary<int, PacketType>>> idToType = new();

    protected override void InitializeData(ProtocolDataBlob data)
    {
        this.idToType = data.IdToTypeMap;

        this.typeToId = this.idToType.Values
                            .SelectMany(x => x.Values)
                            .SelectMany(x => x.ToArray())
                            .ToDictionary(x => x.Value, x => x.Key);
    }

    public int GetPacketId(PacketType type)
    {
        if (!this.Loaded)
            this.Load();

        return this.typeToId[type];
    }

    public PacketType GetPacketType(PacketFlow flow, GameState state, int id)
    {
        if (!this.Loaded)
            this.Load();

        return this.idToType[flow][state][id];
    }
}

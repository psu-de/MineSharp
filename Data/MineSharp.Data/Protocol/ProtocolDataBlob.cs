using MineSharp.Core.Common.Protocol;

namespace MineSharp.Data.Protocol;

internal record ProtocolDataBlob(
    Dictionary<PacketFlow, Dictionary<GameState, Dictionary<int, PacketType>>> IdToTypeMap);

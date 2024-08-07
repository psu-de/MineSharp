using System.Collections.Frozen;
using MineSharp.Core.Common.Protocol;

namespace MineSharp.Data.Protocol;

internal record ProtocolDataBlob(
    FrozenDictionary<PacketFlow, FrozenDictionary<GameState, FrozenDictionary<int, PacketType>>> IdToTypeMap);

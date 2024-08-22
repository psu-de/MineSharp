using MineSharp.Core;
using MineSharp.Core.Exceptions;

namespace MineSharp.Protocol.Exceptions;

/// <summary>
///     Thrown when a packet has invalid data for it's Minecraft version.
/// </summary>
public class MineSharpPacketVersionException(string valueName, ProtocolVersion protocol)
    : MineSharpException($"expected {valueName} to be set for protocol version {protocol}");

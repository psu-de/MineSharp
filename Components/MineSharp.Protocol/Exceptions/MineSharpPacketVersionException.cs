using MineSharp.Core.Exceptions;

namespace MineSharp.Protocol.Exceptions;

/// <summary>
/// Thrown when a packet has invalid data for it's minecraft version.
/// </summary>
public class MineSharpPacketVersionException(string valueName, int protocol)
    : MineSharpException($"expected {valueName} to be set for protocol version {protocol}");

using MineSharp.Core.Exceptions;

namespace MineSharp.Protocol.Exceptions;

/// <summary>
/// Thrown when a packet has invalid data for it's minecraft version.
/// </summary>
/// <param name="message"></param>
public class PacketVersionException(string message) : MineSharpException(message);

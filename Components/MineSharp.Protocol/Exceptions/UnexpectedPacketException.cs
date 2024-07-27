using MineSharp.Core.Exceptions;

namespace MineSharp.Protocol.Exceptions;

/// <summary>
///     Thrown when a packet was unexpected.
/// </summary>
/// <param name="message"></param>
public class UnexpectedPacketException(string message) : MineSharpException(message);

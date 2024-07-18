namespace MineSharp.Core.Common.Protocol;

/// <summary>
///     Specifies the direction of a packet
/// </summary>
public enum PacketFlow
{
    /// <summary>
    ///     Packets sent to the client by the server
    /// </summary>
    Clientbound,

    /// <summary>
    ///     Packets sent to the server by the client
    /// </summary>
    Serverbound
}

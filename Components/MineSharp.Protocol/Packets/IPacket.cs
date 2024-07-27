using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets;

/// <summary>
///     Represents a minecraft packet
/// </summary>
public interface IPacket
{
    /// <summary>
    ///     The corresponding <see cref="PacketType" />
    /// </summary>
    public PacketType Type { get; }

    /// <summary>
    ///     Serialize the packet data into the buffer.
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="version"></param>
    public void Write(PacketBuffer buffer, MinecraftData version);

    /// <summary>
    ///     Read the packet from the buffer
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static abstract IPacket Read(PacketBuffer buffer, MinecraftData version);
}

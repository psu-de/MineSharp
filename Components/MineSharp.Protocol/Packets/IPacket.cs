using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets;

/// <summary>
///     Represents a Minecraft packet
/// </summary>
public interface IPacket
{
    /// <summary>
    ///     The corresponding <see cref="PacketType" />
    ///     
    /// <seealso cref="StaticType"/>
    /// </summary>
    public PacketType Type { get; }

    /// <summary>
    ///     The corresponding <see cref="PacketType" />.
    ///     The same as <see cref="Type" /> but static.
    /// </summary>
    public static abstract PacketType StaticType { get; }

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

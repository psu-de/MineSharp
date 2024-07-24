using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;

/// <summary>
///     Ping packet
///     See https://wiki.vg/Protocol#Ping_.28configuration.29
/// </summary>
public class PingPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Configuration_Ping;
    
    /// <summary>
    ///     The id of the ping
    /// </summary>
    public required int Id { get; init; }

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteInt(Id);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new PingPacket() { Id = buffer.ReadInt() };
    }
}

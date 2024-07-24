using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;

/// <summary>
///     Keep alive packet in Configuration
///     See https://wiki.vg/Protocol#Clientbound_Keep_Alive_.28configuration.29
/// </summary>
public class KeepAlivePacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Configuration_KeepAlive;
    
    /// <summary>
    ///     The keep alive id
    /// </summary>
    public required long KeepAliveId { get; init; }

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteLong(KeepAliveId);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new KeepAlivePacket() { KeepAliveId = buffer.ReadLong() };
    }
}

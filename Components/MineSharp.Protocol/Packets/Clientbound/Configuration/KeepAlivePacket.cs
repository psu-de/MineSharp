using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;

/// <summary>
///     Keep alive packet in Configuration
///     See https://wiki.vg/Protocol#Clientbound_Keep_Alive_.28configuration.29
/// </summary>
/// <param name="KeepAliveId">The keep alive id</param>
public sealed record KeepAlivePacket(long KeepAliveId) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Configuration_KeepAlive;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteLong(KeepAliveId);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new KeepAlivePacket(buffer.ReadLong());
    }
}

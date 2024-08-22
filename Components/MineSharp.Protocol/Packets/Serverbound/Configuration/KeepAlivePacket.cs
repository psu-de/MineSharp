using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Configuration;

/// <summary>
///     Keep alive packet
/// </summary>
/// <param name="KeepAliveId">The keep-alive ID</param>
public sealed partial record KeepAlivePacket(long KeepAliveId) : IPacketStatic<KeepAlivePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Configuration_KeepAlive;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteLong(KeepAliveId);
    }

    /// <inheritdoc />
    public static KeepAlivePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return new KeepAlivePacket(buffer.ReadLong());
    }
}


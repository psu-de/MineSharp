using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;

/// <summary>
///     Ping packet
///     See https://wiki.vg/Protocol#Ping_.28configuration.29
/// </summary>
/// <param name="Id">The id of the ping</param>
public sealed record PingPacket(int Id) : IPacketStatic<PingPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Configuration_Ping;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteInt(Id);
    }

    /// <inheritdoc />
    public static PingPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return new PingPacket(buffer.ReadInt());
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}

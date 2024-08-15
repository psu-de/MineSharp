using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Status;

/// <summary>
///     Packet for ping response
/// </summary>
/// <param name="Payload">The payload of the ping response</param>
public sealed record PingResponsePacket(long Payload) : IPacketStatic<PingResponsePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Status_Ping;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteLong(Payload);
    }

    /// <inheritdoc />
    public static PingResponsePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return new PingResponsePacket(buffer.ReadLong());
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}

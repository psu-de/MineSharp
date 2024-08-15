using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Status;
#pragma warning disable CS1591
/// <summary>
///     Packet for ping request
/// </summary>
/// <param name="Payload">The payload of the ping request</param>
public sealed record PingRequestPacket(long Payload) : IPacketStatic<PingRequestPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Status_Ping;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteLong(Payload);
    }

    /// <inheritdoc />
    public static PingRequestPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return new PingRequestPacket(buffer.ReadLong());
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
#pragma warning restore CS1591

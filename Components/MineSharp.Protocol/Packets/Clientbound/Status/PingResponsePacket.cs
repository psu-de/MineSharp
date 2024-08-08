using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Status;
#pragma warning disable CS1591
/// <summary>
///     Packet for ping response
/// </summary>
/// <param name="Payload">The payload of the ping response</param>
public sealed record PingResponsePacket(long Payload) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Status_Ping;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteLong(Payload);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new PingResponsePacket(buffer.ReadLong());
    }
}
#pragma warning restore CS1591

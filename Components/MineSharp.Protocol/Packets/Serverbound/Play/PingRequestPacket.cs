using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Ping request packet sent by the client to the server.
/// </summary>
/// <param name="Payload">The payload, which may be any number. Notchian clients use a system-dependent time value counted in milliseconds.</param>
public sealed record PingRequestPacket(long Payload) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_PingRequest;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteLong(Payload);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var payload = buffer.ReadLong();

        return new PingRequestPacket(payload);
    }
}

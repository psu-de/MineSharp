using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
/// <summary>
///     Keep alive packet
/// </summary>
/// <param name="KeepAliveId">The keep-alive ID</param>
public sealed record KeepAlivePacket(long KeepAliveId) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_KeepAlive;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteLong(KeepAliveId);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var id = buffer.ReadLong();
        return new KeepAlivePacket(id);
    }
}

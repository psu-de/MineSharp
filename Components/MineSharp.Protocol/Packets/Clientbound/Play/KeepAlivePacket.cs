using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
/// <summary>
///     Keep alive packet
/// </summary>
/// <param name="KeepAliveId">The keep-alive ID</param>
public sealed partial record KeepAlivePacket(long KeepAliveId) : IPacketStatic<KeepAlivePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_KeepAlive;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteLong(KeepAliveId);
    }

    /// <inheritdoc />
    public static KeepAlivePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var id = buffer.ReadLong();
        return new KeepAlivePacket(id);
    }
}

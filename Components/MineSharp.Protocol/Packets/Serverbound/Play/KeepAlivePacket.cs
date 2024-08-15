using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public sealed record KeepAlivePacket(long KeepAliveId) : IPacketStatic<KeepAlivePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_KeepAlive;

    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteLong(KeepAliveId);
    }

    public static KeepAlivePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var id = buffer.ReadLong();
        return new KeepAlivePacket(id);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
#pragma warning restore CS1591

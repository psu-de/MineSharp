using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class KeepAlivePacket : IPacket
{
    public KeepAlivePacket(long id)
    {
        KeepAliveId = id;
    }

    public long KeepAliveId { get; set; }
    public PacketType Type => PacketType.CB_Play_KeepAlive;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteLong(KeepAliveId);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var id = buffer.ReadLong();
        return new KeepAlivePacket(id);
    }
}
#pragma warning restore CS1591

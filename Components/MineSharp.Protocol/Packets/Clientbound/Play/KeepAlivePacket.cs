using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class KeepAlivePacket : IPacket
{
    public static int Id => 0x23;

    public long KeepAliveId { get; set; }

    public KeepAlivePacket(long id)
    {
        this.KeepAliveId = id;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteLong(this.KeepAliveId);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        var id = buffer.ReadLong();
        return new KeepAlivePacket(id);
    }
}

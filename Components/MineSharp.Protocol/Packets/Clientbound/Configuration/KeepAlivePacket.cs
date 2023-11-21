using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;

public class KeepAlivePacket : IPacket
{
    public PacketType Type => PacketType.CB_Configuration_KeepAlive;
    
    public long KeepAliveId { get; set; }
    
    public KeepAlivePacket(long keepAliveId)
    {
        this.KeepAliveId = keepAliveId;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteLong(this.KeepAliveId);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new KeepAlivePacket(buffer.ReadLong());
    }
}

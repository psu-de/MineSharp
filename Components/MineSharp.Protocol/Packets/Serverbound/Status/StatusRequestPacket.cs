using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Serverbound.Status;

public class StatusRequestPacket : IPacket
{
    public static int Id => 0x00;

    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    { }
    
    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        return new StatusRequestPacket();
    }
}

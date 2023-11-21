using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Status;

public class StatusRequestPacket : IPacket
{
    public PacketType Type => PacketType.SB_Status_PingStart;

    public void Write(PacketBuffer buffer, MinecraftData version)
    { }
    
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new StatusRequestPacket();
    }
}

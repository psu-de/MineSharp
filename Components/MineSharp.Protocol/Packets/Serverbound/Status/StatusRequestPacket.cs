using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Status;
#pragma warning disable CS1591
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
#pragma warning restore CS1591

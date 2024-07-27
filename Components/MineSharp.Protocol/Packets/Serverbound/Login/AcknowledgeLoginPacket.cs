using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Login;
#pragma warning disable CS1591
public class AcknowledgeLoginPacket : IPacket
{
    public PacketType Type => PacketType.SB_Login_LoginAcknowledged;

    public void Write(PacketBuffer buffer, MinecraftData version)
    { }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new AcknowledgeLoginPacket();
    }
}
#pragma warning restore CS1591

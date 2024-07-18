using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Status;
#pragma warning disable CS1591
public class StatusResponsePacket : IPacket
{
    public StatusResponsePacket(string response)
    {
        Response = response;
    }

    public string Response { get; set; }
    public PacketType Type => PacketType.CB_Status_ServerInfo;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(Response);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new StatusResponsePacket(buffer.ReadString());
    }
}
#pragma warning restore CS1591

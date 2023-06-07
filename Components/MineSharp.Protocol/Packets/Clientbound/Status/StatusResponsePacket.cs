using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Clientbound.Status;

public class StatusResponsePacket : IPacket
{
    public static int Id => 0x00;

    public string Response { get; set; }

    public StatusResponsePacket(string response)
    {
        this.Response = response;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteString(this.Response);            
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        return new StatusResponsePacket(buffer.ReadString());
    }
}

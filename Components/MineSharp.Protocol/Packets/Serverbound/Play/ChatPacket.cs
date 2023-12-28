using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
/// <summary>
/// ChatPacket used before 1.19 to send a Chat message
/// </summary>
public class ChatPacket : IPacket
{
    public PacketType Type => PacketType.SB_Play_Chat;

    public string Message { get; set; }
    
    public ChatPacket(string message)
    {
        this.Message = message;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(this.Message);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new ChatPacket(buffer.ReadString());
    }
}
#pragma warning restore CS1591
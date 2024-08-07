using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
/// <summary>
///     ChatPacket used before 1.19 to send a Chat message
/// </summary>
public class ChatPacket : IPacket
{
    public ChatPacket(string message)
    {
        Message = message;
    }

    public string Message { get; set; }
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.SB_Play_Chat;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(Message);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new ChatPacket(buffer.ReadString());
    }
}
#pragma warning restore CS1591

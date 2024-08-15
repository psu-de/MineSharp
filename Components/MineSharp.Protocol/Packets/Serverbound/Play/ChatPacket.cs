using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
/// <summary>
///     ChatPacket used before 1.19 to send a Chat message
/// </summary>
public sealed record ChatPacket(string Message) : IPacketStatic<ChatPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_Chat;

    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteString(Message);
    }

    public static ChatPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return new ChatPacket(buffer.ReadString());
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
#pragma warning restore CS1591

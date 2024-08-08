using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
/// <summary>
///     ChatPacket used before 1.19 to send a Chat message
/// </summary>
public sealed record ChatPacket(string Message) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
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

using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     ChatPacket only used for versions &lt;= 1.18.2.
///     It was replaced by multiple packets in 1.19
/// </summary>
/// <param name="Message">The chat message</param>
/// <param name="Position">The position of the chat message</param>
/// <param name="Sender">The UUID of the message sender</param>
public sealed record ChatPacket(string Message, byte Position, Uuid Sender) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_Chat;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(Message);
        buffer.WriteByte(Position);
        buffer.WriteUuid(Sender);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new ChatPacket(
            buffer.ReadString(),
            buffer.ReadByte(),
            buffer.ReadUuid());
    }
}


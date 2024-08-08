using MineSharp.ChatComponent;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Disguised chat message packet
/// </summary>
/// <param name="Message">The chat message</param>
/// <param name="ChatType">The type of chat</param>
/// <param name="Name">The name associated with the chat</param>
/// <param name="Target">The target of the chat message (optional)</param>
public sealed record DisguisedChatMessagePacket(Chat Message, int ChatType, Chat Name, Chat? Target) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_ProfilelessChat;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteChatComponent(Message);
        buffer.WriteVarInt(ChatType);
        buffer.WriteChatComponent(Name);
        buffer.WriteBool(Target != null);
        if (Target != null)
        {
            buffer.WriteChatComponent(Target);
        }
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new DisguisedChatMessagePacket(
            buffer.ReadChatComponent(),
            buffer.ReadVarInt(),
            buffer.ReadChatComponent(),
            buffer.ReadBool() ? buffer.ReadChatComponent() : null);
    }
}

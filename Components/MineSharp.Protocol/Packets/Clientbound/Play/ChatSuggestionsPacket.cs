using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using static MineSharp.Protocol.Packets.Clientbound.Play.ChatSuggestionsPacket;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Chat suggestions packet
/// </summary>
/// <param name="Action">The action to perform</param>
/// <param name="Count">Number of elements in the following array</param>
/// <param name="Entries">Array of chat suggestions</param>
public sealed partial record ChatSuggestionsPacket(ChatSuggestionAction Action, int Count, string[] Entries) : IPacketStatic<ChatSuggestionsPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_ChatSuggestions;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt((int)Action);
        buffer.WriteVarInt(Count);
        buffer.WriteVarIntArray(Entries, (buf, entry) => buf.WriteString(entry));
    }

    /// <inheritdoc />
    public static ChatSuggestionsPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var action = (ChatSuggestionAction)buffer.ReadVarInt();
        var count = buffer.ReadVarInt();
        var entries = buffer.ReadVarIntArray(buf => buf.ReadString());

        return new ChatSuggestionsPacket(action, count, entries);
    }

    /// <summary>
    ///     Enum representing the action for chat suggestions
    /// </summary>
    public enum ChatSuggestionAction
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Add = 0,
        Remove = 1,
        Set = 2
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}

using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Edit Book packet sent by the client to edit or sign a book.
/// </summary>
/// <param name="Slot">The hotbar slot where the written book is located</param>
/// <param name="Entries">Text from each page. Maximum array size is 200. Maximum string length is 8192 chars.</param>
/// <param name="Title">Title of the book. Only present if the book is being signed.</param>
public sealed partial record EditBookPacket(int Slot, string[] Entries, string? Title) : IPacketStatic<EditBookPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_EditBook;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(Slot);
        buffer.WriteVarIntArray(Entries, (buf, entry) => buf.WriteString(entry));
        var hasTitle = Title != null;
        buffer.WriteBool(hasTitle);
        if (hasTitle)
        {
            buffer.WriteString(Title!);
        }
    }

    /// <inheritdoc />
    public static EditBookPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var slot = buffer.ReadVarInt();
        var entries = buffer.ReadVarIntArray(buf => buf.ReadString());
        var hasTitle = buffer.ReadBool();
        var title = hasTitle ? buffer.ReadString() : null;

        return new(slot, entries, title);
    }
}

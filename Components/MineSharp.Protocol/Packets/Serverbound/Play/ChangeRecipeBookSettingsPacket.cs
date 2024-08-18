using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using static MineSharp.Protocol.Packets.Serverbound.Play.ChangeRecipeBookSettingsPacket;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Packet sent by the client to change recipe book settings.
/// </summary>
/// <param name="BookId">The ID of the recipe book.</param>
/// <param name="BookOpen">Whether the book is open.</param>
/// <param name="FilterActive">Whether the filter is active.</param>
public sealed record ChangeRecipeBookSettingsPacket(RecipeBookType BookId, bool BookOpen, bool FilterActive) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_RecipeBook;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt((int)BookId);
        buffer.WriteBool(BookOpen);
        buffer.WriteBool(FilterActive);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var bookId = (RecipeBookType)buffer.ReadVarInt();
        var bookOpen = buffer.ReadBool();
        var filterActive = buffer.ReadBool();

        return new ChangeRecipeBookSettingsPacket(bookId, bookOpen, filterActive);
    }

    /// <summary>
    ///     Enum representing the different types of recipe books.
    /// </summary>
    public enum RecipeBookType
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Crafting = 0,
        Furnace = 1,
        BlastFurnace = 2,
        Smoker = 3
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}

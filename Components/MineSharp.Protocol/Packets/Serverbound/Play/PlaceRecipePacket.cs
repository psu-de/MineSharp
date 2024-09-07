using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Place Recipe packet sent when a player clicks a recipe in the crafting book that is craftable (white border).
/// </summary>
/// <param name="WindowId">The window ID</param>
/// <param name="Recipe">The recipe ID</param>
/// <param name="MakeAll">Whether to make all items (true if shift is down when clicked)</param>
public sealed record PlaceRecipePacket(byte WindowId, Identifier Recipe, bool MakeAll) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_CraftRecipeRequest;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteByte(WindowId);
        buffer.WriteIdentifier(Recipe);
        buffer.WriteBool(MakeAll);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var windowId = buffer.ReadByte();
        var recipe = buffer.ReadIdentifier();
        var makeAll = buffer.ReadBool();

        return new PlaceRecipePacket(windowId, recipe, makeAll);
    }
}

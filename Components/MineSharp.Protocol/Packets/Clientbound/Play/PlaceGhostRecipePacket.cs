using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Response to the serverbound packet (Place Recipe), with the same recipe ID. Appears to be used to notify the UI.
/// </summary>
/// <param name="WindowId">The window ID</param>
/// <param name="Recipe">A recipe ID</param>
public sealed record PlaceGhostRecipePacket(byte WindowId, Identifier Recipe) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_CraftRecipeResponse;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteByte(WindowId);
        buffer.WriteIdentifier(Recipe);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var windowId = buffer.ReadByte();
        var recipe = buffer.ReadIdentifier();

        return new PlaceGhostRecipePacket(windowId, recipe);
    }
}

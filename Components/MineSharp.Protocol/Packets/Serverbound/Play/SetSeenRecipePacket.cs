using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Packet sent by the client when a recipe is first seen in the recipe book.
/// </summary>
/// <param name="RecipeId">The ID of the recipe.</param>
public sealed record SetSeenRecipePacket(Identifier RecipeId) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_DisplayedRecipe;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteIdentifier(RecipeId);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var recipeId = buffer.ReadIdentifier();

        return new SetSeenRecipePacket(recipeId);
    }
}

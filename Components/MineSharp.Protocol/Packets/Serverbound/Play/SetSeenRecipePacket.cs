using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Packet sent by the client when a recipe is first seen in the recipe book.
/// </summary>
/// <param name="RecipeId">The ID of the recipe.</param>
public sealed record SetSeenRecipePacket(Identifier RecipeId) : IPacketStatic<SetSeenRecipePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_DisplayedRecipe;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteIdentifier(RecipeId);
    }

    /// <inheritdoc />
    public static SetSeenRecipePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var recipeId = buffer.ReadIdentifier();

        return new(recipeId);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}

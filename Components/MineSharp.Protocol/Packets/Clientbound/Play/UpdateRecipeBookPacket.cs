using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using static MineSharp.Protocol.Packets.Clientbound.Play.UpdateRecipeBookPacket;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Packet sent by the server to update the recipe book.
/// </summary>
/// <param name="Action">The action to perform (init, add, remove).</param>
/// <param name="CraftingRecipeBookOpen">If true, the crafting recipe book will be open when the player opens its inventory.</param>
/// <param name="CraftingRecipeBookFilterActive">If true, the filtering option is active when the player opens its inventory.</param>
/// <param name="SmeltingRecipeBookOpen">If true, the smelting recipe book will be open when the player opens its inventory.</param>
/// <param name="SmeltingRecipeBookFilterActive">If true, the filtering option is active when the player opens its inventory.</param>
/// <param name="BlastFurnaceRecipeBookOpen">If true, the blast furnace recipe book will be open when the player opens its inventory.</param>
/// <param name="BlastFurnaceRecipeBookFilterActive">If true, the filtering option is active when the player opens its inventory.</param>
/// <param name="SmokerRecipeBookOpen">If true, the smoker recipe book will be open when the player opens its inventory.</param>
/// <param name="SmokerRecipeBookFilterActive">If true, the filtering option is active when the player opens its inventory.</param>
/// <param name="RecipeIds">List of recipe IDs.</param>
/// <param name="OptionalRecipeIds">Optional list of recipe IDs, only present if action is Init.</param>
public sealed record UpdateRecipeBookPacket(
    RecipeBookAction Action,
    bool CraftingRecipeBookOpen,
    bool CraftingRecipeBookFilterActive,
    bool SmeltingRecipeBookOpen,
    bool SmeltingRecipeBookFilterActive,
    bool BlastFurnaceRecipeBookOpen,
    bool BlastFurnaceRecipeBookFilterActive,
    bool SmokerRecipeBookOpen,
    bool SmokerRecipeBookFilterActive,
    Identifier[] RecipeIds,
    Identifier[]? OptionalRecipeIds) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_UnlockRecipes;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt((int)Action);
        buffer.WriteBool(CraftingRecipeBookOpen);
        buffer.WriteBool(CraftingRecipeBookFilterActive);
        buffer.WriteBool(SmeltingRecipeBookOpen);
        buffer.WriteBool(SmeltingRecipeBookFilterActive);
        buffer.WriteBool(BlastFurnaceRecipeBookOpen);
        buffer.WriteBool(BlastFurnaceRecipeBookFilterActive);
        buffer.WriteBool(SmokerRecipeBookOpen);
        buffer.WriteBool(SmokerRecipeBookFilterActive);
        buffer.WriteVarInt(RecipeIds.Length);
        foreach (var recipeId in RecipeIds)
        {
            buffer.WriteIdentifier(recipeId);
        }
        if (Action == RecipeBookAction.Init)
        {
            buffer.WriteVarInt(OptionalRecipeIds?.Length ?? 0);
            if (OptionalRecipeIds != null)
            {
                foreach (var optionalRecipeId in OptionalRecipeIds)
                {
                    buffer.WriteIdentifier(optionalRecipeId);
                }
            }
        }
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var action = (RecipeBookAction)buffer.ReadVarInt();
        var craftingRecipeBookOpen = buffer.ReadBool();
        var craftingRecipeBookFilterActive = buffer.ReadBool();
        var smeltingRecipeBookOpen = buffer.ReadBool();
        var smeltingRecipeBookFilterActive = buffer.ReadBool();
        var blastFurnaceRecipeBookOpen = buffer.ReadBool();
        var blastFurnaceRecipeBookFilterActive = buffer.ReadBool();
        var smokerRecipeBookOpen = buffer.ReadBool();
        var smokerRecipeBookFilterActive = buffer.ReadBool();
        var recipeIdsLength = buffer.ReadVarInt();
        var recipeIds = new Identifier[recipeIdsLength];
        for (int i = 0; i < recipeIdsLength; i++)
        {
            recipeIds[i] = buffer.ReadIdentifier();
        }
        Identifier[]? optionalRecipeIds = null;
        if (action == RecipeBookAction.Init)
        {
            var optionalRecipeIdsLength = buffer.ReadVarInt();
            optionalRecipeIds = new Identifier[optionalRecipeIdsLength];
            for (int i = 0; i < optionalRecipeIdsLength; i++)
            {
                optionalRecipeIds[i] = buffer.ReadIdentifier();
            }
        }
        return new UpdateRecipeBookPacket(
            action,
            craftingRecipeBookOpen,
            craftingRecipeBookFilterActive,
            smeltingRecipeBookOpen,
            smeltingRecipeBookFilterActive,
            blastFurnaceRecipeBookOpen,
            blastFurnaceRecipeBookFilterActive,
            smokerRecipeBookOpen,
            smokerRecipeBookFilterActive,
            recipeIds,
            optionalRecipeIds);
    }

    /// <summary>
    ///     Enum representing the action to perform on the recipe book.
    /// </summary>
    public enum RecipeBookAction
    {
        /// <summary>
        /// All the recipes in list 1 will be tagged as displayed, and all the recipes in list 2 will be added to the recipe book. Recipes that aren't tagged will be shown in the notification.
        /// </summary>
        Init = 0,
        /// <summary>
        /// All the recipes in the list are added to the recipe book and their icons will be shown in the notification.
        /// </summary>
        Add = 1,
        /// <summary>
        /// Remove all the recipes in the list. This allows them to be re-displayed when they are re-added.
        /// </summary>
        Remove = 2
    }
}

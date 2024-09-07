using System.Collections.Frozen;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Items;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;
using static MineSharp.Protocol.Packets.Clientbound.Play.UpdateRecipesPacket;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
/// Represents a packet sent by the server to update the list of recipes.
/// </summary>
public sealed record UpdateRecipesPacket(
    Recipe[] Recipes
) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_DeclareRecipes;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(Recipes.Length);
        foreach (var recipe in Recipes)
        {
            recipe.Write(buffer, data);
        }
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var numRecipes = buffer.ReadVarInt();
        var recipes = new Recipe[numRecipes];
        for (int i = 0; i < numRecipes; i++)
        {
            recipes[i] = Recipe.Read(buffer, data);
        }
        return new UpdateRecipesPacket(recipes);
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface IUpdateRecipesData
    {
        public void Write(PacketBuffer buffer, MinecraftData data);
    }

    public interface IUpdateRecipesDataStatic
    {
        public static abstract IUpdateRecipesData Read(PacketBuffer buffer, MinecraftData data);
    }

    public static class UpdateRecipesDataRegistry
    {
        public static IUpdateRecipesData Read(PacketBuffer buffer, MinecraftData data, Identifier type)
        {
            if (!UpdateRecipesDataTypeFactories.TryGetValue(type, out var reader))
            {
                throw new InvalidOperationException($"Unsupported data type: {type}");
            }
            return reader(buffer, data);
        }

        public static readonly FrozenDictionary<Identifier, Func<PacketBuffer, MinecraftData, IUpdateRecipesData>> UpdateRecipesDataTypeFactories;

        static UpdateRecipesDataRegistry()
        {
            UpdateRecipesDataTypeFactories = InitializeUpdateRecipesDataTypes();
        }

        private static FrozenDictionary<Identifier, Func<PacketBuffer, MinecraftData, IUpdateRecipesData>> InitializeUpdateRecipesDataTypes()
        {
            var dict = new Dictionary<Identifier, Func<PacketBuffer, MinecraftData, IUpdateRecipesData>>();

            void Register<T>(params Identifier[] identifiers)
                where T : IUpdateRecipesData, IUpdateRecipesDataStatic
            {
                var factory = T.Read;
                foreach (var identifier in identifiers)
                {
                    dict.Add(identifier, factory);
                }
            }

            // TODO: Does this data come from some registry?
            Register<ShapelessCraftingData>(
                Identifier.Parse("minecraft:crafting_shapeless")
            );
            Register<ShapedCraftingData>(
                Identifier.Parse("minecraft:crafting_shaped")
            );
            Register<SpecialCraftingData>(
                Identifier.Parse("minecraft:crafting_special_armordye"),
                Identifier.Parse("minecraft:crafting_special_bookcloning"),
                Identifier.Parse("minecraft:crafting_special_mapcloning"),
                Identifier.Parse("minecraft:crafting_special_mapextending"),
                Identifier.Parse("minecraft:crafting_special_firework_rocket"),
                Identifier.Parse("minecraft:crafting_special_firework_star"),
                Identifier.Parse("minecraft:crafting_special_firework_star_fade"),
                Identifier.Parse("minecraft:crafting_special_repairitem"),
                Identifier.Parse("minecraft:crafting_special_tippedarrow"),
                Identifier.Parse("minecraft:crafting_special_bannerduplicate"),
                Identifier.Parse("minecraft:crafting_special_shielddecoration"),
                Identifier.Parse("minecraft:crafting_special_shulkerboxcoloring"),
                Identifier.Parse("minecraft:crafting_special_suspiciousstew"),
                Identifier.Parse("minecraft:crafting_decorated_pot")
            );
            Register<SmeltingData>(
                Identifier.Parse("minecraft:smelting"),
                Identifier.Parse("minecraft:blasting"),
                Identifier.Parse("minecraft:smoking"),
                Identifier.Parse("minecraft:campfire_cooking")
            );
            Register<StonecuttingData>(
                Identifier.Parse("minecraft:stonecutting")
            );
            Register<SmithingTransformData>(
                Identifier.Parse("minecraft:smithing_transform")
            );
            Register<SmithingTrimData>(
                Identifier.Parse("minecraft:smithing_trim")
            );

            return dict.ToFrozenDictionary();
        }
    }

    /// <summary>
    /// Represents a recipe in the update recipes packet.
    /// </summary>
    public sealed record Recipe(
        Identifier Type,
        Identifier RecipeId,
        IUpdateRecipesData Data
    ) : ISerializableWithMinecraftData<Recipe>
    {
        /// <inheritdoc />
        public void Write(PacketBuffer buffer, MinecraftData data)
        {
            buffer.WriteIdentifier(Type);
            buffer.WriteIdentifier(RecipeId);
            Data.Write(buffer, data);
        }

        /// <inheritdoc />
        public static Recipe Read(PacketBuffer buffer, MinecraftData data)
        {
            var type = buffer.ReadIdentifier();
            var recipeId = buffer.ReadIdentifier();
            var recipeData = UpdateRecipesDataRegistry.Read(buffer, data, type);
            return new Recipe(type, recipeId, recipeData);
        }
    }

    public enum CraftingCategory
    {
        Building,
        Redstone,
        Equipment,
        Misc
    }

    public sealed record ShapelessCraftingData(
        string Group,
        CraftingCategory Category,
        Ingredient[] Ingredients,
        Item Result
    ) : IUpdateRecipesData, IUpdateRecipesDataStatic, ISerializableWithMinecraftData<ShapelessCraftingData>
    {
        /// <inheritdoc />
        public void Write(PacketBuffer buffer, MinecraftData data)
        {
            buffer.WriteString(Group);
            buffer.WriteVarInt((int)Category);
            buffer.WriteVarInt(Ingredients.Length);
            foreach (var ingredient in Ingredients)
            {
                ingredient.Write(buffer, data);
            }
            buffer.WriteOptionalItem(Result);
        }

        /// <inheritdoc />
        public static ShapelessCraftingData Read(PacketBuffer buffer, MinecraftData data)
        {
            var group = buffer.ReadString();
            var category = (CraftingCategory)buffer.ReadVarInt();
            var ingredientCount = buffer.ReadVarInt();
            var ingredients = new Ingredient[ingredientCount];
            for (int i = 0; i < ingredientCount; i++)
            {
                ingredients[i] = Ingredient.Read(buffer, data);
            }
            var result = buffer.ReadOptionalItem(data)!;
            return new ShapelessCraftingData(group, category, ingredients, result);
        }

        static IUpdateRecipesData IUpdateRecipesDataStatic.Read(PacketBuffer buffer, MinecraftData data)
        {
            return Read(buffer, data);
        }
    }

    public sealed record ShapedCraftingData(
        string Group,
        CraftingCategory Category,
        int Width,
        int Height,
        Ingredient[] Ingredients,
        Item Result,
        bool ShowNotification
    ) : IUpdateRecipesData, IUpdateRecipesDataStatic, ISerializableWithMinecraftData<ShapedCraftingData>
    {
        public void Write(PacketBuffer buffer, MinecraftData data)
        {
            buffer.WriteString(Group);
            buffer.WriteVarInt((int)Category);
            buffer.WriteVarInt(Width);
            buffer.WriteVarInt(Height);
            foreach (var ingredient in Ingredients)
            {
                ingredient.Write(buffer, data);
            }
            buffer.WriteOptionalItem(Result);
            buffer.WriteBool(ShowNotification);
        }

        public static ShapedCraftingData Read(PacketBuffer buffer, MinecraftData data)
        {
            var group = buffer.ReadString();
            var category = (CraftingCategory)buffer.ReadVarInt();
            var width = buffer.ReadVarInt();
            var height = buffer.ReadVarInt();
            var ingredients = new Ingredient[width * height];
            for (int i = 0; i < ingredients.Length; i++)
            {
                ingredients[i] = Ingredient.Read(buffer, data);
            }
            var result = buffer.ReadOptionalItem(data)!;
            var showNotification = buffer.ReadBool();
            return new ShapedCraftingData(group, category, width, height, ingredients, result, showNotification);
        }

        static IUpdateRecipesData IUpdateRecipesDataStatic.Read(PacketBuffer buffer, MinecraftData data)
        {
            return Read(buffer, data);
        }
    }

    public sealed record SpecialCraftingData(
        CraftingCategory Category
    ) : IUpdateRecipesData, IUpdateRecipesDataStatic, ISerializableWithMinecraftData<SpecialCraftingData>
    {
        public void Write(PacketBuffer buffer, MinecraftData data)
        {
            buffer.WriteVarInt((int)Category);
        }

        public static SpecialCraftingData Read(PacketBuffer buffer, MinecraftData data)
        {
            var category = (CraftingCategory)buffer.ReadVarInt();
            return new SpecialCraftingData(category);
        }

        static IUpdateRecipesData IUpdateRecipesDataStatic.Read(PacketBuffer buffer, MinecraftData data)
        {
            return Read(buffer, data);
        }
    }

    public enum SmeltingCraftingCategory
    {
        Food,
        Blocks,
        Misc
    }

    public sealed record SmeltingData(
        string Group,
        SmeltingCraftingCategory Category,
        Ingredient Ingredient,
        Item Result,
        float Experience,
        int CookingTime
    ) : IUpdateRecipesData, IUpdateRecipesDataStatic, ISerializableWithMinecraftData<SmeltingData>
    {
        public void Write(PacketBuffer buffer, MinecraftData data)
        {
            buffer.WriteString(Group);
            buffer.WriteVarInt((int)Category);
            Ingredient.Write(buffer, data);
            buffer.WriteOptionalItem(Result);
            buffer.WriteFloat(Experience);
            buffer.WriteVarInt(CookingTime);
        }

        public static SmeltingData Read(PacketBuffer buffer, MinecraftData data)
        {
            var group = buffer.ReadString();
            var category = (SmeltingCraftingCategory)buffer.ReadVarInt();
            var ingredient = Ingredient.Read(buffer, data);
            var result = buffer.ReadOptionalItem(data)!;
            var experience = buffer.ReadFloat();
            var cookingTime = buffer.ReadVarInt();
            return new SmeltingData(group, category, ingredient, result, experience, cookingTime);
        }

        static IUpdateRecipesData IUpdateRecipesDataStatic.Read(PacketBuffer buffer, MinecraftData data)
        {
            return Read(buffer, data);
        }
    }

    public sealed record StonecuttingData(
        string Group,
        Ingredient Ingredient,
        Item Result
    ) : IUpdateRecipesData, IUpdateRecipesDataStatic, ISerializableWithMinecraftData<StonecuttingData>
    {
        public void Write(PacketBuffer buffer, MinecraftData data)
        {
            buffer.WriteString(Group);
            Ingredient.Write(buffer, data);
            buffer.WriteOptionalItem(Result);
        }

        public static StonecuttingData Read(PacketBuffer buffer, MinecraftData data)
        {
            var group = buffer.ReadString();
            var ingredient = Ingredient.Read(buffer, data);
            var result = buffer.ReadOptionalItem(data)!;
            return new StonecuttingData(group, ingredient, result);
        }

        static IUpdateRecipesData IUpdateRecipesDataStatic.Read(PacketBuffer buffer, MinecraftData data)
        {
            return Read(buffer, data);
        }
    }

    public sealed record SmithingTransformData(
        Ingredient Template,
        Ingredient Base,
        Ingredient Addition,
        Item Result
    ) : IUpdateRecipesData, IUpdateRecipesDataStatic, ISerializableWithMinecraftData<SmithingTransformData>
    {
        public void Write(PacketBuffer buffer, MinecraftData data)
        {
            Template.Write(buffer, data);
            Base.Write(buffer, data);
            Addition.Write(buffer, data);
            buffer.WriteOptionalItem(Result);
        }

        public static SmithingTransformData Read(PacketBuffer buffer, MinecraftData data)
        {
            var template = Ingredient.Read(buffer, data);
            var baseItem = Ingredient.Read(buffer, data);
            var addition = Ingredient.Read(buffer, data);
            var result = buffer.ReadOptionalItem(data);
            return new SmithingTransformData(template, baseItem, addition, result);
        }

        static IUpdateRecipesData IUpdateRecipesDataStatic.Read(PacketBuffer buffer, MinecraftData data)
        {
            return Read(buffer, data);
        }
    }

    public sealed record SmithingTrimData(
        Ingredient Template,
        Ingredient Base,
        Ingredient Addition
    ) : IUpdateRecipesData, IUpdateRecipesDataStatic, ISerializableWithMinecraftData<SmithingTrimData>
    {
        public void Write(PacketBuffer buffer, MinecraftData data)
        {
            Template.Write(buffer, data);
            Base.Write(buffer, data);
            Addition.Write(buffer, data);
        }

        public static SmithingTrimData Read(PacketBuffer buffer, MinecraftData data)
        {
            var template = Ingredient.Read(buffer, data);
            var baseItem = Ingredient.Read(buffer, data);
            var addition = Ingredient.Read(buffer, data);
            return new SmithingTrimData(template, baseItem, addition);
        }

        static IUpdateRecipesData IUpdateRecipesDataStatic.Read(PacketBuffer buffer, MinecraftData data)
        {
            return Read(buffer, data);
        }
    }

    public sealed record Ingredient(
        Item[] Items
    ) : ISerializableWithMinecraftData<Ingredient>
    {
        public void Write(PacketBuffer buffer, MinecraftData data)
        {
            buffer.WriteVarInt(Items.Length);
            foreach (var item in Items)
            {
                buffer.WriteOptionalItem(item);
            }
        }

        public static Ingredient Read(PacketBuffer buffer, MinecraftData data)
        {
            var count = buffer.ReadVarInt();
            var items = new Item[count];
            for (int i = 0; i < count; i++)
            {
                items[i] = buffer.ReadOptionalItem(data)!;
            }
            return new Ingredient(items);
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}

using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Effects;
using MineSharp.Core.Common.Enchantments;
using MineSharp.Core.Common.Entities;
using MineSharp.Core.Common.Items;
using MineSharp.Data.Biomes;
using MineSharp.Data.BlockCollisionShapes;
using MineSharp.Data.Blocks;
using MineSharp.Data.Effects;
using MineSharp.Data.Enchantments;
using MineSharp.Data.Entities;
using MineSharp.Data.Items;
using MineSharp.Data.Language;
using MineSharp.Data.Materials;
using MineSharp.Data.Protocol;
using MineSharp.Data.Recipes;
using MineSharp.Data.Windows;

namespace MineSharp.Data;

public class MinecraftData
{
    public BiomeProvider Biomes { get; }
    public BlockProvider Blocks { get; }
    public BlockCollisionShapesProvider BlockCollisionShapes { get; }
    public EffectProvider Effects { get; }
    public EnchantmentProvider Enchantments { get; }
    public EntityProvider Entities { get; }
    public ItemProvider Items { get; }
    public ProtocolProvider Protocol { get; }
    public MaterialsProvider Materials { get; }
    public RecipeProvider Recipes { get; }
    public WindowData Windows { get; } = new WindowData();
    public LanguageProvider Language { get; }
    public MinecraftVersion Version { get; }

    private MinecraftData(
        BiomeProvider biomes,
        BlockProvider blocks,
        BlockCollisionShapesProvider blockCollisionShapes,
        EffectProvider effects,
        EnchantmentProvider enchantments,
        EntityProvider entities,
        ItemProvider items,
        ProtocolProvider protocol,
        MaterialsProvider materials,
        RecipeProvider recipes,
        LanguageProvider language,
        MinecraftVersion version)
    {
        this.Biomes = biomes;
        this.Blocks = blocks;
        this.BlockCollisionShapes = blockCollisionShapes;
        this.Effects = effects;
        this.Enchantments = enchantments;
        this.Entities = entities;
        this.Items = items;
        this.Protocol = protocol;
        this.Materials = materials;
        this.Recipes = recipes;
        this.Language = language;
        this.Version = version;
    }

    public static MinecraftData FromVersion(string version)
    {
        var biomeType = GetClassType(VersionMap.Biomes[version]);
        var blockType = GetClassType(VersionMap.Blocks[version]);
        var blockCollisionShapeType = GetClassType(VersionMap.BlockCollisionShapes[version]);
        var effectType = GetClassType(VersionMap.Effects[version]);
        var enchantmentType = GetClassType(VersionMap.Enchantments[version]);
        var entityType = GetClassType(VersionMap.Entities[version]);
        var itemType = GetClassType(VersionMap.Items[version]);
        var protocolType = GetClassType(VersionMap.Protocol[version]);
        var materialType = GetClassType(VersionMap.Materials[version]);
        var recipeType = GetClassType(VersionMap.Recipes[version]);
        var languageType = GetClassType(VersionMap.Language[version]);

        var biomes = new BiomeProvider(
            (DataVersion<BiomeType, BiomeInfo>)Activator.CreateInstance(biomeType)!);
        var blocks = new BlockProvider(
            (DataVersion<BlockType, BlockInfo>)Activator.CreateInstance(blockType)!);
        var blockCollisionShapes = new BlockCollisionShapesProvider(
            (BlockCollisionShapesVersion)Activator.CreateInstance(blockCollisionShapeType)!);
        var effects = new EffectProvider(
            (DataVersion<EffectType, EffectInfo>)Activator.CreateInstance(effectType)!);
        var enchantments = new EnchantmentProvider(
            (DataVersion<EnchantmentType, EnchantmentInfo>)Activator.CreateInstance(enchantmentType)!);
        var entities = new EntityProvider(
            (DataVersion<EntityType, EntityInfo>)Activator.CreateInstance(entityType)!);
        var items = new ItemProvider(
            (DataVersion<ItemType, ItemInfo>)Activator.CreateInstance(itemType)!);
        var materials = new MaterialsProvider(
            (MaterialVersion)Activator.CreateInstance(materialType)!);
        var protocol = new ProtocolProvider(
            (ProtocolVersion)Activator.CreateInstance(protocolType)!);
        var recipes = new RecipeProvider(
            (RecipeData)Activator.CreateInstance(recipeType)!);
        var language = new LanguageProvider(
            (LanguageVersion)Activator.CreateInstance(languageType)!);

        return new MinecraftData(
            biomes,
            blocks,
            blockCollisionShapes,
            effects,
            enchantments,
            entities,
            items,
            protocol,
            materials,
            recipes,
            language,
            VersionMap.Versions[version]);
    }

    private static Type GetClassType(string className)
    {
        var type = Type.GetType(className);
        if (null == type)
            throw new NotImplementedException($"{className} is not implemented.");

        return type;
    }
}

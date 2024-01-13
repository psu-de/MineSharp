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
using MineSharp.Data.Windows.Versions;

namespace MineSharp.Data;

/// <summary>
/// Provides static data about a Minecraft version.
/// </summary>
public class MinecraftData
{
    /// <summary>
    /// The Biome data provider for this version
    /// </summary>
    public BiomeProvider Biomes { get; }
    
    /// <summary>
    /// The Block data provider for this version
    /// </summary>
    public BlockProvider Blocks { get; }
    
    /// <summary>
    /// The Collision shape data provider for this version
    /// </summary>
    public BlockCollisionShapesProvider BlockCollisionShapes { get; }
    
    /// <summary>
    /// The effect data provider for this version
    /// </summary>
    public EffectProvider Effects { get; }
    
    /// <summary>
    /// The enchantment data provider for this version
    /// </summary>
    public EnchantmentProvider Enchantments { get; }
    
    /// <summary>
    /// The entity data provider for this version
    /// </summary>
    public EntityProvider Entities { get; }
    
    /// <summary>
    /// The item data provider for this version
    /// </summary>
    public ItemProvider Items { get; }
    
    /// <summary>
    /// The protocol data provider for this version
    /// </summary>
    public ProtocolProvider Protocol { get; }
    
    /// <summary>
    /// The material data provider for this version
    /// </summary>
    public MaterialsProvider Materials { get; }
    
    /// <summary>
    /// The recipe data provider for this version
    /// </summary>
    public RecipeProvider Recipes { get; }
    
    /// <summary>
    /// The window data for this version
    /// </summary>
    public WindowProvider Windows { get; }
    
    /// <summary>
    /// The language data provider for this version
    /// </summary>
    public LanguageProvider Language { get; }
    
    /// <summary>
    /// The minecraft version of this instance
    /// </summary>
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
        WindowProvider windows,
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
        this.Windows = windows;
        this.Language = language;
        this.Version = version;
    }

    
    /// <summary>
    /// Returns a MinecraftData object for the given version.
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
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

        var v = VersionMap.Versions[version];

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
        var windows = new WindowProvider(
            v.Protocol > 764 
                ? new WindowVersion1_20_3() 
                : new WindowVersion1_16_1()
        );

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
            windows,
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

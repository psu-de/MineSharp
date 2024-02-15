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
using MineSharp.Data.Exceptions;
using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Items;
using MineSharp.Data.Materials;
using MineSharp.Data.Protocol;
using Newtonsoft.Json.Linq;
using NLog;

namespace MineSharp.Data;

/// <summary>
/// Provides static data about a Minecraft version.
/// </summary>
public class MinecraftData : IMinecraftData
{
    private static readonly MinecraftDataRepository MinecraftDataRepository =
        new (
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MineSharp",
                "MinecraftData"),
            new HttpClient()); 
    
    /// <summary>
    /// The Biome data provider for this version
    /// </summary>
    public IBiomeData Biomes { get; }
    
    /// <summary>
    /// The Block data provider for this version
    /// </summary>
    public IBlockData Blocks { get; }
    
    /// <summary>
    /// The Collision shape data provider for this version
    /// </summary>
    public IBlockCollisionShapeData BlockCollisionShapes { get; }
    
    /// <summary>
    /// The effect data provider for this version
    /// </summary>
    public IEffectData Effects { get; }
    
    /// <summary>
    /// The enchantment data provider for this version
    /// </summary>
    public IEnchantmentData Enchantments { get; }
    
    /// <summary>
    /// The entity data provider for this version
    /// </summary>
    public IEntityData Entities { get; }
    
    /// <summary>
    /// The item data provider for this version
    /// </summary>
    public IItemData Items { get; }
    
    /// <summary>
    /// The protocol data provider for this version
    /// </summary>
    public IProtocolData Protocol { get; }
    
    /// <summary>
    /// The material data provider for this version
    /// </summary>
    public IMaterialData Materials { get; }
    
    /// <summary>
    /// The recipe data provider for this version
    /// </summary>
    public IRecipeData Recipes { get; }
    
    /// <summary>
    /// The window data for this version
    /// </summary>
    public IWindowData Windows { get; }
    
    /// <summary>
    /// The language data provider for this version
    /// </summary>
    public ILanguageData Language { get; }
    
    /// <summary>
    /// The minecraft version of this instance
    /// </summary>
    public MinecraftVersion Version { get; }

    private MinecraftData(
        IDataProvider<BiomeInfo[]> biomes,
        IDataProvider<BlockInfo[]> blocks,
        IDataProvider<BlockCollisionShapeDataBlob> blockCollisionShapes,
        IDataProvider<EffectInfo[]> effects,
        IDataProvider<EnchantmentInfo[]> enchantments,
        IDataProvider<EntityInfo[]> entities,
        IDataProvider<ItemInfo[]> items,
        IDataProvider<ProtocolDataBlob> protocol,
        IDataProvider<MaterialDataBlob> materials,
        IDataProvider<object> recipes,
        IDataProvider<object> windows,
        IDataProvider<object> language,
        MinecraftVersion version)
    {
        this.Biomes = new BiomeData(biomes);
        this.Blocks = new BlockData(blocks);
        this.BlockCollisionShapes = new BlockCollisionShapeData(blockCollisionShapes, this);
        this.Effects = new EffectData(effects);
        this.Enchantments = new EnchantmentData(enchantments);
        this.Entities = new EntityData(entities);
        this.Items = new ItemData(items);
        this.Protocol = new ProtocolData(protocol);
        this.Materials = new MaterialData(materials);
        this.Recipes = null;
        this.Windows = null;
        this.Language = null;
        this.Version = version;
    }
    
    /// <summary>
    /// Returns a MinecraftData object for the given version.
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public static async Task<MinecraftData> FromVersion(string version)
    {
        var resourceMap = await MinecraftDataRepository.GetResourceMap();

        var versionToken = resourceMap.SelectToken($"pc.{version}");
        if (null == versionToken)
            throw new MineSharpVersionNotSupportedException($"Version {version} is not supported.");

        var biomeToken = await LoadAsset("biomes", versionToken);
        var shapesToken = await LoadAsset("blockCollisionShapes", versionToken);
        
        
        var biomeProvider = new BiomeProvider(biomeToken);
        var shapesProvider = new BlockCollisionShapesProvider(shapesToken);

        return new MinecraftData(
            biomeProvider,
            null,
            shapesProvider,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null);
    }

    private static async Task<JToken> LoadAsset(string resource, JToken version)
    {
        return await MinecraftDataRepository.GetAsset(
            $"{resource}.json", 
            (string)version.SelectToken(resource)!);
    }
}

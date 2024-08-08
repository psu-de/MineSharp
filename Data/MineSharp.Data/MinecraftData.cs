using MineSharp.Core.Common;
using MineSharp.Data.Biomes;
using MineSharp.Data.BlockCollisionShapes;
using MineSharp.Data.Blocks;
using MineSharp.Data.Effects;
using MineSharp.Data.Enchantments;
using MineSharp.Data.Entities;
using MineSharp.Data.Exceptions;
using MineSharp.Data.Framework;
using MineSharp.Data.Items;
using MineSharp.Data.Language;
using MineSharp.Data.Materials;
using MineSharp.Data.Protocol;
using MineSharp.Data.Recipes;
using MineSharp.Data.Windows;
using MineSharp.Data.Windows.Versions;
using Newtonsoft.Json.Linq;
using NLog;

namespace MineSharp.Data;

/// <summary>
///     Provides static data about a Minecraft version.
/// </summary>
public class MinecraftData
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private static readonly MinecraftDataRepository MinecraftDataRepository =
        new(
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MineSharp",
                "MinecraftData"),
            new());

    private static readonly Lazy<Dictionary<string, JToken>> ProtocolVersions = new(LoadProtocolVersions);
    private static readonly Dictionary<string, MinecraftData> LoadedData = new();

    private MinecraftData(
        IBiomeData biomes,
        IBlockData blocks,
        IBlockCollisionShapeData blockCollisionShapes,
        IEffectData effects,
        IEnchantmentData enchantments,
        IEntityData entities,
        IItemData items,
        IProtocolData protocol,
        IMaterialData materials,
        IRecipeData recipes,
        IWindowData windows,
        ILanguageData language,
        MinecraftVersion version)
    {
        Biomes = biomes;
        Blocks = blocks;
        BlockCollisionShapes = blockCollisionShapes;
        Effects = effects;
        Enchantments = enchantments;
        Entities = entities;
        Items = items;
        Protocol = protocol;
        Materials = materials;
        Recipes = recipes;
        Windows = windows;
        Language = language;
        Version = version;
    }

    /// <summary>
    ///     The Biome data provider for this version
    /// </summary>
    public IBiomeData Biomes { get; }

    /// <summary>
    ///     The Block data provider for this version
    /// </summary>
    public IBlockData Blocks { get; }

    /// <summary>
    ///     The Collision shape data provider for this version
    /// </summary>
    public IBlockCollisionShapeData BlockCollisionShapes { get; }

    /// <summary>
    ///     The effect data provider for this version
    /// </summary>
    public IEffectData Effects { get; }

    /// <summary>
    ///     The enchantment data provider for this version
    /// </summary>
    public IEnchantmentData Enchantments { get; }

    /// <summary>
    ///     The entity data provider for this version
    /// </summary>
    public IEntityData Entities { get; }

    /// <summary>
    ///     The item data provider for this version
    /// </summary>
    public IItemData Items { get; }

    /// <summary>
    ///     The protocol data provider for this version
    /// </summary>
    public IProtocolData Protocol { get; }

    /// <summary>
    ///     The material data provider for this version
    /// </summary>
    public IMaterialData Materials { get; }

    /// <summary>
    ///     The recipe data provider for this version
    /// </summary>
    public IRecipeData Recipes { get; }

    /// <summary>
    ///     The window data for this version
    /// </summary>
    public IWindowData Windows { get; }

    /// <summary>
    ///     The language data provider for this version
    /// </summary>
    public ILanguageData Language { get; }

    /// <summary>
    ///     The Minecraft version of this instance
    /// </summary>
    public MinecraftVersion Version { get; }

    /// <summary>
    ///     Returns a MinecraftData object for the given version.
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public static async Task<MinecraftData> FromVersion(string version)
    {
        if (LoadedData.TryGetValue(version, out var loaded))
        {
            return loaded;
        }

        var versionToken = await TryGetVersion(version);

        if (versionToken is null)
        {
            throw new MineSharpVersionNotSupportedException($"Version {version} is not supported.");
        }

        var protocolVersion = (int)ProtocolVersions.Value[version].SelectToken("version")!;
        var minecraftVersion = new MinecraftVersion(version, protocolVersion);

        var biomeToken = await LoadAsset("biomes", versionToken);
        var shapesToken = await LoadAsset("blockCollisionShapes", versionToken);
        var blocksToken = await LoadAsset("blocks", versionToken);
        var itemsToken = await LoadAsset("items", versionToken);
        var effectsToken = await LoadAsset("effects", versionToken);
        var enchantmentsToken = await LoadAsset("enchantments", versionToken);
        var entitiesToken = await LoadAsset("entities", versionToken);
        var protocolToken = await LoadAsset("protocol", versionToken);
        var materialsToken = await LoadAsset("materials", versionToken);
        var recipesToken = await LoadAsset("recipes", versionToken);
        var languageToken = await LoadAsset("language", versionToken);

        var biomes = new BiomeData(new BiomeProvider(biomeToken));
        var items = new ItemData(new ItemProvider(itemsToken));
        var blocks = new BlockData(new BlockProvider(blocksToken, items));
        var shapes = new BlockCollisionShapeData(new BlockCollisionShapesProvider(shapesToken));
        var effects = new EffectData(new EffectProvider(effectsToken));
        var enchantments = new EnchantmentData(new EnchantmentProvider(enchantmentsToken));
        var entities = new EntityData(new EntityProvider(entitiesToken));
        var protocol = new ProtocolData(new ProtocolProvider(protocolToken));
        var materials = new MaterialData(new MaterialsProvider(materialsToken, items));
        var recipes = new RecipeData(new(recipesToken, items));
        var language = new LanguageData(new LanguageProvider(languageToken));
        var windows = GetWindowData(minecraftVersion);

        var data = new MinecraftData(
            biomes,
            blocks,
            shapes,
            effects,
            enchantments,
            entities,
            items,
            protocol,
            materials,
            recipes,
            windows,
            language,
            minecraftVersion);

        LoadedData.Add(version, data);
        return data;
    }

    private static Task<JToken> LoadAsset(string resource, JToken version)
    {
        return MinecraftDataRepository.GetAsset(
            $"{resource}.json",
            (string)version.SelectToken(resource)!);
    }

    private static async Task<JToken?> TryGetVersion(string version)
    {
        var resourceMap = await MinecraftDataRepository.GetResourceMap();

        var versionToken = resourceMap["pc"]?[version];
        if (versionToken is not null)
        {
            return versionToken;
        }

        if (!ProtocolVersions.Value.TryGetValue(version, out var token))
        {
            return null;
        }

        var majorVersion = (string)token.SelectToken("majorVersion")!;
        return resourceMap["pc"]?[majorVersion];
    }

    private static Dictionary<string, JToken> LoadProtocolVersions()
    {
        var protocolVersions = (JArray)MinecraftDataRepository.GetAsset(
                                                                   "protocolVersions.json",
                                                                   "pc/common")
                                                              .Result;

        return protocolVersions
              .DistinctBy(x => (string)x.SelectToken("minecraftVersion")!)
              .ToDictionary(
                   x => (string)x.SelectToken("minecraftVersion")!,
                   x => x);
    }

    private static IWindowData GetWindowData(MinecraftVersion version)
    {
        return version.Protocol switch
        {
            >= 765 => new(new WindowVersion1203()),
            >= 736 => new WindowData(new WindowVersion1161()),
            _ => throw new NotSupportedException()
        };
    }
}

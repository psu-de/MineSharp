using MineSharp.Core.Common;
using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Effects;
using MineSharp.Core.Common.Enchantments;
using MineSharp.Core.Common.Entities;
using MineSharp.Core.Common.Items;
using MineSharp.Core.Common.Particles;
using MineSharp.Core.Registries;
using MineSharp.Data.BlockCollisionShapes;
using MineSharp.Data.Exceptions;
using MineSharp.Data.Framework;
using MineSharp.Data.Language;
using MineSharp.Data.Materials;
using MineSharp.Data.Protocol;
using MineSharp.Data.Recipes;
using MineSharp.Data.Utils;
using Newtonsoft.Json.Linq;
using NLog;

namespace MineSharp.Data;

/// <summary>
///     Provides static data about a Minecraft version.
/// </summary>
public class MinecraftData
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private static readonly GitHubRepositoryHelper MinecraftDataRepository = new("PrismarineJs/minecraft-data");
    private static readonly GitHubRepositoryHelper McInfoRepository = new("MineSharp-NET/mcinfo");

    private static readonly Lazy<Dictionary<string, JToken>> ProtocolVersions = new(LoadProtocolVersions);
    private static readonly Dictionary<string, MinecraftData> LoadedData = new();
    
    private MinecraftData(
        Registries registries,
        IBlockCollisionShapeData blockCollisionShapes,
        IProtocolData protocol,
        IMaterialData materials,
        IRecipeData recipes,
        ILanguageData language,
        MinecraftVersion version)
    {
        Registries = registries;
        BlockCollisionShapes = blockCollisionShapes;
        Protocol = protocol;
        Materials = materials;
        Recipes = recipes;
        Language = language;
        Registries = registries;
        Version = version;
        
        // for convenience, the most common registries have their own field in MinecraftData
        Biomes = registries.Biomes;
        Blocks = registries.Blocks;
        Effects = registries.Effects;
        Enchantments = registries.Enchantments;
        Entities = registries.Entities;
        Items = registries.Items;
        Particles = registries.Particles;
        Menus = registries.Menus;
    }

    /// <summary>
    /// All client- and server-side registries for minecraft:vanilla
    /// </summary>
    public Registries Registries { get; }
    
    
    /// <inheritdoc cref="Data.Registries.Biomes"/>
    public BiomeRegistry Biomes { get; }
    
    /// <inheritdoc cref="Data.Registries.Blocks"/>
    public BlockRegistry Blocks { get; }
    
    /// <inheritdoc cref="Data.Registries.Effects"/>
    public EffectRegistry Effects { get; }
    
    /// <inheritdoc cref="Data.Registries.Enchantments"/>
    public EnchantmentRegistry Enchantments { get; }
    
    /// <inheritdoc cref="Data.Registries.Entities"/>
    public EntityRegistry Entities { get; }
    
    /// <inheritdoc cref="Data.Registries.Items"/>
    public ItemRegistry Items { get; }
    
    /// <inheritdoc cref="Data.Registries.Particles"/>
    public ParticleRegistry Particles { get; }
    
    /// <inheritdoc cref="Data.Registries.Menus"/>
    public Registry<RegistryResource> Menus { get; }

    /// <summary>
    ///     The Collision shape data provider for this version
    /// </summary>
    public IBlockCollisionShapeData BlockCollisionShapes { get; }

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

        var minecraftDataResourceMap = await TryGetMinecraftDataResourceMap(version);
        var mcinfoResourceMap = await TryGetMcInfoResourceMap(version);

        if (mcinfoResourceMap is null)
        {
            throw new MineSharpVersionNotSupportedException($"Version {version} is not supported by mcinfo.");
        }
        
        if (minecraftDataResourceMap is null)
        {
            throw new MineSharpVersionNotSupportedException($"Version {version} is not supported by minecraft-data.");
        }

        var protocolVersion = (int)ProtocolVersions.Value[version].SelectToken("version")!;
        var minecraftVersion = new MinecraftVersion(version, protocolVersion);

        var shapesToken = await LoadMinecraftDataAsset("blockCollisionShapes", minecraftDataResourceMap);
        var protocolToken = await LoadMinecraftDataAsset("protocol", minecraftDataResourceMap);
        var materialsToken = await LoadMinecraftDataAsset("materials", minecraftDataResourceMap);
        var recipesToken = await LoadMinecraftDataAsset("recipes", minecraftDataResourceMap);
        var languageToken = await LoadMinecraftDataAsset("language", minecraftDataResourceMap);
        var registriesToken = await LoadMcInfoAsset("registries", mcinfoResourceMap);
        
        var parser = new DataParser();
        var biomes = await RegistryHelper.LoadRegistry<BiomeRegistry, BiomeInfo, BiomeType>(
            () => LoadMinecraftDataAsset("biomes", minecraftDataResourceMap), 
            parser.ParseBiome);

        var items = await RegistryHelper.LoadRegistry<ItemRegistry, ItemInfo, ItemType>(
            () => LoadMinecraftDataAsset("items", minecraftDataResourceMap),
            parser.ParseItem);

        var blocks = await RegistryHelper.LoadRegistry<BlockRegistry, BlockInfo, BlockType, int>(
            () => LoadMinecraftDataAsset("blocks", minecraftDataResourceMap),
            x => parser.ParseBlock(x, items),
            x => (int)x.SelectToken("maxStateId")!); // order blocks by max state because BlockRegistry#ByState requires it

        var effects = await RegistryHelper.LoadRegistry<EffectRegistry, EffectInfo, EffectType>(
            () => LoadMinecraftDataAsset("effects", minecraftDataResourceMap),
            parser.ParseEffect);

        var enchantments = await RegistryHelper.LoadRegistry<EnchantmentRegistry, EnchantmentInfo, EnchantmentType>(
            () => LoadMinecraftDataAsset("enchantments", minecraftDataResourceMap),
            parser.ParseEnchantment);
        
        var entities = await RegistryHelper.LoadRegistry<EntityRegistry, EntityInfo, EntityType>(
            () => LoadMinecraftDataAsset("entities", minecraftDataResourceMap),
            parser.ParseEntity);
        
        var particles = await RegistryHelper.LoadRegistry<ParticleRegistry, RegistryResource<ParticleType>, ParticleType>(
            () => LoadMinecraftDataAsset("particles", minecraftDataResourceMap),
            parser.ParseParticle);
        
        var registries = new Registries
        {
            [biomes.Name] = biomes,
            [blocks.Name] = blocks,
            [items.Name] = items,
            [effects.Name] = effects,
            [enchantments.Name] = enchantments,
            [entities.Name] = entities,
            [particles.Name] = particles
        };

        ParseAdditionalRegistries(registries, registriesToken);
        
        var shapes = new BlockCollisionShapeData(new BlockCollisionShapesProvider(shapesToken));
        var protocol = new ProtocolData(new ProtocolProvider(protocolToken));
        var materials = new MaterialData(new MaterialsProvider(materialsToken, items));
        var recipes = new RecipeData(new(recipesToken, items));
        var language = new LanguageData(new LanguageProvider(languageToken));

        var data = new MinecraftData(
            registries,
            shapes,
            protocol,
            materials,
            recipes,
            language,
            minecraftVersion);

        LoadedData.Add(version, data);
        return data;
    }

    private static Task<JToken> LoadMinecraftDataAsset(string resource, JToken version)
    {
        var versionPath = (string)version.SelectToken(resource)!;
        var fullyQualifiedName = $"data/{versionPath}/{resource}.json";
        
        return MinecraftDataRepository.GetAsset(fullyQualifiedName);
    }

    private static Task<JToken> LoadMcInfoAsset(string resource, JToken resourceMap)
    {
        var filepath = (string?)resourceMap.SelectToken(resource);

        if (string.IsNullOrEmpty(filepath))
        {
            throw new InvalidOperationException($"Data {resource} is not available.");
        }
        
        return McInfoRepository.GetAsset(filepath, "main");
    }

    private static async Task<JToken?> TryGetMinecraftDataResourceMap(string version)
    {
        var resourceMap = await MinecraftDataRepository.GetAsset("data/dataPaths.json");

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

    private static async Task<JToken?> TryGetMcInfoResourceMap(string version)
    {
        var resourceMap = await McInfoRepository.GetAsset("summary.json", "main");

        return resourceMap[version];
    }

    private static Dictionary<string, JToken> LoadProtocolVersions()
    {
        var protocolVersions = (JArray)MinecraftDataRepository.GetAsset("data/pc/common/protocolVersions.json")
                                                              .Result;

        return protocolVersions
              .DistinctBy(x => (string)x.SelectToken("minecraftVersion")!)
              .ToDictionary(
                   x => (string)x.SelectToken("minecraftVersion")!,
                   x => x);
    }

    private static void ParseAdditionalRegistries(Registries registries, JToken token)
    {
        var obj = (JObject)token;

        foreach (var property in obj.Properties())
        {
            var name = Identifier.Parse(property.Name);
            if (registries.ContainsKey(name))
            {
                continue;
            }
            
            var registry = new Registry<RegistryResource>(name);

            foreach (var entry in ((JObject)property.Value).Properties())
            {
                var resource = new RegistryResource(Identifier.Parse(entry.Name), (int)entry.Value);
                registry.Register(resource);
            }

            registries.Add(registry);
        }
    }
}

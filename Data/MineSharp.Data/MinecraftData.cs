using MineSharp.Core.Cache;
using MineSharp.Data.Exceptions;
using MineSharp.Data.Json;
using MineSharp.Data.Providers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;

namespace MineSharp.Data;

public class MinecraftData
{
    public const string CACHE_NAME = "Data";

    public BiomeDataProvider Biomes { get; }
    public BlockDataProvider Blocks { get; }
    public CollisionDataProvider Collisions { get; }
    public EffectDataProvider Effects { get; }
    public EnchantmentDataProvider Enchantments { get; }
    public EntityDataProvider Entities { get; }
    public ItemDataProvider Items { get; }
    public RecipeDataProvider Recipes { get; }
    public LanguageDataProvider Language { get; }
    public ProtocolDataProvider Protocol { get; }
    public FeatureProvider Features { get; }

    public MinecraftVersion Version { get; }
    
    private MinecraftData(GenerateInfoBlob gi, LoadDataConfig loadConfig, VersionInfoBlob? version = null)
    {
        var info = version ?? JsonConvert.DeserializeObject<VersionInfoBlob>(
            File.ReadAllText(MinecraftDataRepo.GetJsonFile(gi.VersionPath, "version")))!;

        this.Version = new MinecraftVersion(info.MinecraftVersion);

        this.Biomes = new BiomeDataProvider(MinecraftDataRepo.GetJsonFile(gi.BiomesPath, "biomes"));
        
        this.Blocks = new BlockDataProvider(
            MinecraftDataRepo.GetJsonFile(gi.BlocksPath, "blocks"), 
            MinecraftDataRepo.GetJsonFile(gi.BlockCollisionShapesPath, "blockCollisionShapes"));
        
        this.Collisions = new CollisionDataProvider(MinecraftDataRepo.GetJsonFile(gi.BlockCollisionShapesPath, "blockCollisionShapes"));
        this.Effects = new EffectDataProvider(MinecraftDataRepo.GetJsonFile(gi.EffectsPath, "effects"));
        this.Enchantments = new EnchantmentDataProvider(MinecraftDataRepo.GetJsonFile(gi.EnchantmentsPath, "enchantments"));
        this.Entities = new EntityDataProvider(MinecraftDataRepo.GetJsonFile(gi.EntitiesPath, "entities"));
        this.Items = new ItemDataProvider(MinecraftDataRepo.GetJsonFile(gi.ItemsPath, "items"));
        this.Recipes = new RecipeDataProvider(MinecraftDataRepo.GetJsonFile(gi.RecipesPath, "recipes"));
        this.Language = new LanguageDataProvider(MinecraftDataRepo.GetJsonFile(gi.LanguagePath, "language"));
        this.Protocol = new ProtocolDataProvider(MinecraftDataRepo.GetJsonFile(gi.ProtocolPath, "protocol"), info.ProtocolVersion);

        this.Features = new FeatureProvider(
            MinecraftDataRepo.GetJsonFile("pc/common", "features"),
            MinecraftDataRepo.GetJsonFile("pc/common", "versions"), this.Version);
        
        this.ConditionallyLoadData(loadConfig);
    }

    private void ConditionallyLoadData(LoadDataConfig config)
    {
        if (config.LoadBiomes) this.Biomes.Load();
        if (config.LoadBlocks) this.Blocks.Load();
        if (config.LoadCollisionShapes) this.Collisions.Load();
        if (config.LoadEffectData) this.Effects.Load();
        if (config.LoadEnchantmentData) this.Enchantments.Load();
        if (config.LoadEntityData) this.Entities.Load();
        if (config.LoadItemData) this.Items.Load();
        if (config.LoadRecipeData) this.Recipes.Load();
        if (config.LoadLanguageData) this.Language.Load();
        if (config.LoadProtocolData) this.Protocol.Load();
        if (config.LoadFeatures) this.Features.Load();
    }

    private static readonly Dictionary<string, VersionInfoBlob> MajorVersionByVersion = new Dictionary<string, VersionInfoBlob>();
    private static readonly DataPathsBlob DataPaths;
    
    static MinecraftData()
    {
        var cacheFolder = CacheManager.Get(CACHE_NAME);
        MinecraftDataRepo.DownloadIfNecessary(cacheFolder);
        
        string dataPathsPath = MinecraftDataRepo.GetAbsoluteDataPath("dataPaths.json");
        DataPaths = JsonConvert.DeserializeObject<DataPathsBlob>(File.ReadAllText(dataPathsPath));

        
        var protocolVersions = JsonConvert.DeserializeObject<VersionInfoBlob[]>(
            File.ReadAllText(MinecraftDataRepo.GetJsonFile("pc/common", "protocolVersions")))!;

        foreach (var version in protocolVersions)
        {
            MajorVersionByVersion.TryAdd(version.MinecraftVersion, version);
        }
    }

    public static MinecraftData FromVersion(string version) 
        => FromVersion(version, LoadDataConfig.LoadAll);
    
    public static MinecraftData FromVersion(string version, LoadDataConfig loadConfig)
    {
        if (!DataPaths.PCVersions.ContainsKey(version))
        {
            if (!MajorVersionByVersion.TryGetValue(version, out var major))
            {
                throw new MineSharpVersionNotFoundException($"Version ${version} is not supported by minecraft-data.");
            }

            var majorInfo = DataPaths.PCVersions[major.MajorVersion];
            return new MinecraftData(majorInfo, loadConfig, major);
        }

        var info = DataPaths.PCVersions[version];
        return new MinecraftData(info, loadConfig);
    }
}
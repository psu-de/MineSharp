namespace MineSharp.Data;

public class LoadDataConfig
{
    public static readonly LoadDataConfig LoadAll = new LoadDataConfig() {
        LoadBiomes = true,
        LoadBlocks = true,
        LoadCollisionShapes = true,
        LoadEffectData = true,
        LoadEnchantmentData = true,
        LoadEntityData = true,
        LoadItemData = true,
        LoadLanguageData = true,
        LoadProtocolData = true,
        LoadRecipeData = true,
        LoadFeatures = true
    };
    
    public bool LoadBiomes { get; set; }
    public bool LoadBlocks { get; set; }
    public bool LoadCollisionShapes { get; set; }
    public bool LoadEffectData { get; set; }
    public bool LoadEnchantmentData { get; set; }
    public bool LoadEntityData { get; set; }
    public bool LoadItemData { get; set; }
    public bool LoadLanguageData { get; set; }
    public bool LoadProtocolData { get; set; }
    public bool LoadRecipeData { get; set; }
    public bool LoadFeatures { get; set; }

    /// <summary>
    /// Creates a new instance of the <see cref="LoadDataConfig"/> class.
    /// Defaults to loading nothing.
    /// </summary>
    public LoadDataConfig()
    {
        LoadBiomes = false;
        LoadBlocks = false;
        LoadCollisionShapes = false;
        LoadEffectData = false;
        LoadEnchantmentData = false;
        LoadEntityData = false;
        LoadItemData = false;
        LoadLanguageData = false;
        LoadProtocolData = false;
        LoadRecipeData = false;
        LoadFeatures = false;
    }
}

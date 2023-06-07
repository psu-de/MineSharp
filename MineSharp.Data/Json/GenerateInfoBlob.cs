using Newtonsoft.Json;

namespace MineSharp.Data.Json;

internal struct GenerateInfoBlob
{
    [JsonProperty("attributes")]
    public string AttributesPath { get; set; }
    [JsonProperty("blocks")]
    public string BlocksPath { get; set; }
    [JsonProperty("blockCollisionShapes")]
    public string BlockCollisionShapesPath { get; set; }
    [JsonProperty("biomes")]
    public string BiomesPath { get; set; }
    [JsonProperty("enchantments")]
    public string EnchantmentsPath { get; set; }
    [JsonProperty("effects")]
    public string EffectsPath { get; set; }
    [JsonProperty("items")]
    public string ItemsPath { get; set; }
    [JsonProperty("recipes")]
    public string RecipesPath { get; set; }
    [JsonProperty("instruments")]
    public string InstrumentsPath { get; set; }
    [JsonProperty("materials")]
    public string MaterialsPath { get; set; }
    [JsonProperty("entities")]
    public string EntitiesPath { get; set; }
    [JsonProperty("protocol")]
    public string ProtocolPath { get; set; }
    [JsonProperty("windows")]
    public string WindowsPath { get; set; }
    [JsonProperty("version")]
    public string VersionPath { get; set; }
    [JsonProperty("language")]
    public string LanguagePath { get; set; }
    [JsonProperty("foods")]
    public string FoodsPath { get; set; }
    [JsonProperty("particles")]
    public string ParticlesPath { get; set; }
    [JsonProperty("tints")]
    public string TintsPath { get; set; }
    [JsonProperty("mapIcons")]
    public string MapIcons { get; set; }
}

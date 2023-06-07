using Newtonsoft.Json;

namespace MineSharp.Data.Json;

internal class EnchantmentInfoBlob
{
    [JsonProperty("id")]
    public int Id ;

    [JsonProperty("name")]
    public string Name;

    [JsonProperty("displayName")]
    public string DisplayName;

    [JsonProperty("maxLevel")]
    public int MaxLevel;

    [JsonProperty("minCost")]
    public EnchantCostBlob MinCost;

    [JsonProperty("maxCost")]
    public EnchantCostBlob MaxCost;

    [JsonProperty("treasureOnly")]
    public bool TreasureOnly;

    [JsonProperty("curse")]
    public bool Curse;

    [JsonProperty("exclude")]
    public List<string> Exclude;

    [JsonProperty("category")]
    public string Category;

    [JsonProperty("weight")]
    public int Weight;

    [JsonProperty("tradeable")]
    public bool Tradeable;

    [JsonProperty("discoverable")]
    public bool Discoverable;
}

internal class EnchantCostBlob
{
    [JsonProperty("a")]
    public int A;

    [JsonProperty("b")]
    public int B;
}
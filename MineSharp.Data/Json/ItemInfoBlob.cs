using Newtonsoft.Json;

namespace MineSharp.Data.Json;

internal class ItemInfoBlob
{
    [JsonProperty("id")]
    public int Id;

    [JsonProperty("displayName")]
    public string DisplayName;

    [JsonProperty("name")]
    public string Name;

    [JsonProperty("stackSize")]
    public int StackSize;

    [JsonProperty("maxDurability")]
    public int MaxDurability;

    [JsonProperty("enchantCategories")]
    public string[] EnchantCategories;

    [JsonProperty("repairWith")]
    public string[] RepairWith;
}

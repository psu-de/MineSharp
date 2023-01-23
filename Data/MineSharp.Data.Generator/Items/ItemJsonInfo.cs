using Newtonsoft.Json;

namespace MineSharp.Data.Generator.Items
{
#pragma warning disable CS8618
    internal class ItemJsonInfo
    {
        [JsonProperty("id")]
        [Index(0)]
        public int Id;

        [JsonProperty("displayName")]
        [Index(1)]
        public string DisplayName;

        [JsonProperty("name")]
        [Index(2)]
        public string Name;

        [JsonProperty("stackSize")]
        [Index(3)]
        public int StackSize;

        [JsonProperty("maxDurability")]
        [Index(4)]
        public int MaxDurability;

        [JsonProperty("enchantCategories")]
        [Index(5)]
        public string[] EnchantCategories;

        [JsonProperty("repairWith")]
        [Index(6)]
        public string[] RepairWith;
    }
#pragma warning restore CS8618
}

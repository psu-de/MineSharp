using Newtonsoft.Json;
namespace MineSharp.Data.Generator.Items
{
#pragma warning disable CS8618
    internal class ItemJsonInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("stackSize")]
        public int StackSize { get; set; }

        [JsonProperty("maxDurability")]
        public int MaxDurability { get; set; }

        [JsonProperty("enchantCategories")]
        public string[] EnchantCategories { get; set; }

        [JsonProperty("repairWith")]
        public string[] RepairWith { get; set; }
    }
#pragma warning restore CS8618
}

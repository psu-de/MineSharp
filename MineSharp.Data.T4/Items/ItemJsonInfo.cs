using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.T4.Items {
    public class ItemJsonInfo {
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
}

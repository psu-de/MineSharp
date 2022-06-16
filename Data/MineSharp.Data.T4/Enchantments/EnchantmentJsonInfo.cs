using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Enchantments {
    public class MaxCost {
        [JsonProperty("a")]
        public int A { get; set; }

        [JsonProperty("b")]
        public int B { get; set; }
    }

    public class MinCost {
        [JsonProperty("a")]
        public int A { get; set; }

        [JsonProperty("b")]
        public int B { get; set; }
    }

    public class EnchantmentJsonInfo {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("maxLevel")]
        public int MaxLevel { get; set; }

        [JsonProperty("minCost")]
        public MinCost MinCost { get; set; }

        [JsonProperty("maxCost")]
        public MaxCost MaxCost { get; set; }

        [JsonProperty("treasureOnly")]
        public bool TreasureOnly { get; set; }

        [JsonProperty("curse")]
        public bool Curse { get; set; }

        [JsonProperty("exclude")]
        public List<string> Exclude { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("weight")]
        public int Weight { get; set; }

        [JsonProperty("tradeable")]
        public bool Tradeable { get; set; }

        [JsonProperty("discoverable")]
        public bool Discoverable { get; set; }
    }
}

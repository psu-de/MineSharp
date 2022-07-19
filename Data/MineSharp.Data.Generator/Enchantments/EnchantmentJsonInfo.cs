using Newtonsoft.Json;

namespace MineSharp.Data.Generator.Enchantments {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    internal class MaxCost {
        [JsonProperty("a")]
        public int A { get; set; }

        [JsonProperty("b")]
        public int B { get; set; }
    }

    internal class MinCost {
        [JsonProperty("a")]
        public int A { get; set; }

        [JsonProperty("b")]
        public int B { get; set; }
    }

    internal class EnchantmentJsonInfo {
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
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
}

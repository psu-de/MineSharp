using Newtonsoft.Json;

namespace MineSharp.Data.Generator.Enchantments
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    internal class MaxCost
    {
        [JsonProperty("a")]
        public int A { get; set; }

        [JsonProperty("b")]
        public int B { get; set; }
    }

    internal class MinCost
    {
        [JsonProperty("a")]
        public int A { get; set; }

        [JsonProperty("b")]
        public int B { get; set; }
    }

    internal class EnchantmentJsonInfo
    {
        [JsonProperty("id")]
        [Index(0)]
        public int Id ;

        [JsonProperty("name")]
        [Index(1)]
        public string Name;

        [JsonProperty("displayName")]
        [Index(2)]
        public string DisplayName;

        [JsonProperty("maxLevel")]
        [Index(3)]
        public int MaxLevel;

        [JsonProperty("minCost")]
        [Index(4)]
        public MinCost MinCost;

        [JsonProperty("maxCost")]
        [Index(5)]
        public MaxCost MaxCost;

        [JsonProperty("treasureOnly")]
        [Index(6)]
        public bool TreasureOnly;

        [JsonProperty("curse")]
        [Index(7)]
        public bool Curse;

        [JsonProperty("exclude")]
        [Index(8)]
        public List<string> Exclude;

        [JsonProperty("category")]
        [Index(9)]
        public string Category;

        [JsonProperty("weight")]
        [Index(10)]
        public int Weight;

        [JsonProperty("tradeable")]
        [Index(11)]
        public bool Tradeable;

        [JsonProperty("discoverable")]
        [Index(12)]
        public bool Discoverable;
    }
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
}

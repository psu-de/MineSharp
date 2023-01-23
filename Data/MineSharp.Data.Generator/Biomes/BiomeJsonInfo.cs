using Newtonsoft.Json;

namespace MineSharp.Data.Generator.Biomes
{
#pragma warning disable CS8618
#pragma warning disable CS0649
    internal class BiomeJsonInfo
    {
        [JsonProperty("id")]
        [Index(0)]
        public int Id;

        [JsonProperty("name")]
        [Index(1)]
        public string Name;

        [JsonProperty("category")]
        [Index(2)]
        public string Category;

        [JsonProperty("temperature")]
        [Index(3)]
        public float Temperature;

        [JsonProperty("precipitation")]
        [Index(4)]
        public string Precipitation;

        [JsonProperty("depth")]
        [Index(5)]
        public float Depth;

        [JsonProperty("dimension")]
        [Index(7)]
        public string Dimension;

        [JsonProperty("displayName")]
        [Index(8)]
        public string DisplayName;

        [JsonProperty("color")]
        [Index(9)]
        public int Color;

        [JsonProperty("rainfall")]
        [Index(10)]
        public float Rainfall;
    }
#pragma warning restore CS8618
#pragma warning restore CS0649
}

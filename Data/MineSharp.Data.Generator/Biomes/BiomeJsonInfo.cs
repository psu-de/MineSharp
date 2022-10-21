using Newtonsoft.Json;

namespace MineSharp.Data.Generator.Biomes
{
#pragma warning disable CS8618
#pragma warning disable CS0649
    internal class BiomeJsonInfo
    {
        [JsonProperty("category")]
        public string Category;
        [JsonProperty("color")]
        public int Color;
        [JsonProperty("depth")]
        public float Depth;
        [JsonProperty("dimension")]
        public string Dimension;
        [JsonProperty("displayName")]
        public string DisplayName;
        [JsonProperty("id")]
        public int Id;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("precipitation")]
        public string Precipitation;
        [JsonProperty("rainfall")]
        public float Rainfall;
        [JsonProperty("temperature")]
        public float Temperature;
    }
#pragma warning restore CS8618
#pragma warning restore CS0649
}

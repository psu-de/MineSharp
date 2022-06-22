using Newtonsoft.Json;

namespace MineSharp.Data.Biomes {
    public class BiomeJsonInfo {
        [JsonProperty("id")]
        public int Id;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("category")]
        public string Category;
        [JsonProperty("temperature")]
        public float Temperature;
        [JsonProperty("precipitation")]
        public string Precipitation;
        [JsonProperty("depth")]
        public float Depth;
        [JsonProperty("dimension")]
        public string Dimension;
        [JsonProperty("displayName")]
        public string DisplayName;
        [JsonProperty("color")]
        public int Color;
        [JsonProperty("rainfall")]
        public float Rainfall;
    }
}

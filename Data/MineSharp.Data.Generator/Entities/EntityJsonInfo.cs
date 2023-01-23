using Newtonsoft.Json;

namespace MineSharp.Data.Generator.Entities
{
#pragma warning disable CS8618
    internal class EntityJsonInfo
    {
        [JsonProperty("id")]
        [Index(0)]
        public int Id;

        [JsonProperty("internalId")]
        public int InternalId;

        [JsonProperty("name")]
        [Index(1)]
        public string Name;

        [JsonProperty("displayName")]
        [Index(2)]
        public string DisplayName;

        [JsonProperty("width")]
        [Index(3)]
        public float Width;

        [JsonProperty("height")]
        [Index(4)]
        public float Height;

        [JsonProperty("type")]
        public string Type;

        [JsonProperty("category")]
        [Index(5)]
        public string Category;
    }
#pragma warning restore CS8618
}

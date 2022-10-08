using Newtonsoft.Json;
namespace MineSharp.Data.Generator.Entities
{
#pragma warning disable CS8618
    internal class EntityJsonInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("internalId")]
        public int InternalId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("width")]
        public double Width { get; set; }

        [JsonProperty("height")]
        public double Height { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }
    }
#pragma warning restore CS8618
}

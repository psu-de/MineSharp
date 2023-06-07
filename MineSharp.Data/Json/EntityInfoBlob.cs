using Newtonsoft.Json;

namespace MineSharp.Data.Json;

internal class EntityInfoBlob
{
    [JsonProperty("id")]
    public int Id;

    [JsonProperty("internalId")]
    public int InternalId;

    [JsonProperty("name")]
    public string Name;

    [JsonProperty("displayName")]
    public string DisplayName;

    [JsonProperty("width")]
    public float Width;

    [JsonProperty("height")]
    public float Height;

    [JsonProperty("type")]
    public string Type;

    [JsonProperty("category")]
    public string Category;
}

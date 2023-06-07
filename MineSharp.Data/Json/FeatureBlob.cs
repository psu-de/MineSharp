using Newtonsoft.Json;

namespace MineSharp.Data.Json;

public class FeatureBlob
{
    [JsonProperty("name")]
    public string Name;

    [JsonProperty("description")]
    public string? Description;

    [JsonProperty("versions")]
    public string[] Versions;
}

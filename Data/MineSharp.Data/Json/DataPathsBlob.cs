using Newtonsoft.Json;

namespace MineSharp.Data.Json;

internal struct DataPathsBlob
{
    [JsonProperty("pc")]
    public Dictionary<string, GenerateInfoBlob> PCVersions;
    [JsonProperty("bedrock")]
    public Dictionary<string, object> BedrockVersions;
}

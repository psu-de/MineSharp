using Newtonsoft.Json;

namespace MineSharp.Data.Json;

public class VersionInfoBlob
{
    [JsonProperty("minecraftVersion")]
    public string MinecraftVersion;

    [JsonProperty("version")]
    public int ProtocolVersion;

    [JsonProperty("majorVersion")]
    public string MajorVersion;
}

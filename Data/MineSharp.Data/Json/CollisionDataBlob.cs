using Newtonsoft.Json;

namespace MineSharp.Data.Json;

internal class CollisionDataBlob
{
    [JsonProperty("blocks")]
    public Dictionary<string, object> Blocks;

    [JsonProperty("shapes")]
    public Dictionary<int, float[][]> Shapes;
}

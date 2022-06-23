using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Blocks {
#pragma warning disable CS8618
    public class BlockJsonInfo {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("hardness")]
        public float? Hardness { get; set; }

        [JsonProperty("resistance")]
        public float? Resistance { get; set; }

        [JsonProperty("minStateId")]
        public int? MinStateId { get; set; }

        [JsonProperty("maxStateId")]
        public int? MaxStateId { get; set; }

        [JsonProperty("states")]
        public BlockStateJsonInfo[]? States { get; set; }

        [JsonProperty("drops")]
        public object[] Drops { get; set; }

        [JsonProperty("diggable")]
        public bool Diggable { get; set; }

        [JsonProperty("transparent")]
        public bool Transparent { get; set; }

        [JsonProperty("filterLight")]
        public int FilterLight { get; set; }

        [JsonProperty("emitLight")]
        public int EmitLight { get; set; }

        [JsonProperty("boundingBox")]
        public string BoundingBox { get; set; }

        [JsonProperty("stackSize")]
        public int StackSize { get; set; }

        [JsonProperty("material")]
        public string? Material { get; set; }

        [JsonProperty("harvestTools")]
        public Dictionary<string, bool>? HarvestTools { get; set; }

        [JsonProperty("defaultState")]
        public int? DefaultState { get; set; }
    }

    public class BlockStateJsonInfo {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("num_values")]
        public int NumValues { get; set; }
        [JsonProperty("values")]
        public string[]? Values { get; set; }
    }

    public class BlockCollisionShapeJson {
        [JsonProperty("blocks")]
        public Dictionary<string, object> Blocks;
        [JsonProperty("shapes")]
        public Dictionary<string, float[][]> Shapes;


        public static int[] GetShapeIndices(object obj) {
            if (obj is JArray) {
                return ((JArray)obj).Select(x => (int)x).ToArray();
            } else {
                return new int[] { (int)((long)obj) };
            }
        }
    }
#pragma warning restore CS8618
}

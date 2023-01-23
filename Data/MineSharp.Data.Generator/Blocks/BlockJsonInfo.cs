using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Generator.Blocks
{
#pragma warning disable CS8618
    internal class BlockJsonInfo
    {
        [JsonProperty("id")]
        [Index(0)]
        public int Id;

        [JsonProperty("name")]
        [Index(1)]
        public string Name;

        [JsonProperty("displayName")]
        [Index(2)]
        public string DisplayName;


        [JsonProperty("hardness")]
        [Index(3)]
        public float? Hardness;

        [JsonProperty("resistance")]
        [Index(4)]
        public float? Resistance;

        [JsonProperty("diggable")]
        [Index(5)]
        public bool Diggable;

        [JsonProperty("transparent")]
        [Index(6)]
        public bool Transparent;

        [JsonProperty("filterLight")]
        [Index(7)]
        public int FilterLight;

        [JsonProperty("emitLight")]
        [Index(8)]
        public int EmitLight;

        [JsonProperty("boundingBox")]
        [Index(9)]
        public string BoundingBox;

        [JsonProperty("stackSize")]
        [Index(10)]
        public int StackSize;

        [JsonProperty("material")]
        [Index(11)]
        public string? Material;

        [JsonProperty("defaultState")]
        [Index(12)]
        public int? DefaultState;

        [JsonProperty("minStateId")]
        [Index(13)]
        public int? MinStateId;

        [JsonProperty("maxStateId")]
        [Index(14)]
        public int? MaxStateId;

        [JsonProperty("harvestTools")]
        [Index(15)]
        public Dictionary<string, bool>? HarvestTools;

        [JsonProperty("states")]
        [Index(16)]
        public BlockStateJsonInfo[]? States;

        [JsonProperty("drops")]
        public object[] Drops;


    }

    internal class BlockStateJsonInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("num_values")]
        public int NumValues { get; set; }
        [JsonProperty("values")]
        public string[]? Values { get; set; }
    }

    internal class BlockCollisionShapeJson
    {
        [JsonProperty("blocks")]
        public Dictionary<string, object> Blocks;
        [JsonProperty("shapes")]
        public Dictionary<string, float[][]> Shapes;


        public static int[] GetShapeIndices(object obj)
        {
            if (obj is JArray)
            {
                return ((JArray)obj).Select(x => (int)x).ToArray();
            }
            return new[] {
                (int)(long)obj
            };
        }
    }
#pragma warning restore CS8618
}

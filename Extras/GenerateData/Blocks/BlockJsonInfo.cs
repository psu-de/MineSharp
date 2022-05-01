using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateData.Blocks {
    internal class BlockJsonInfo {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("hardness")]
        public float? Hardness { get; set; }

        [JsonProperty("resistance")]
        public float Resistance { get; set; }

        [JsonProperty("minStateId")]
        public int MinStateId { get; set; }

        [JsonProperty("maxStateId")]
        public int MaxStateId { get; set; }

        [JsonProperty("states")]
        public List<object> States { get; set; }

        [JsonProperty("drops")]
        public List<object> Drops { get; set; }

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
        public string Material { get; set; }

        [JsonProperty("harvestTools")]
        public Dictionary<string, bool>? HarvestTools { get; set; }

        [JsonProperty("defaultState")]
        public int DefaultState { get; set; }
    }
}

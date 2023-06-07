using Newtonsoft.Json;

namespace MineSharp.Data.Json;

internal class BlockInfoBlob
{
    [JsonProperty("id")]
    public int Id;
    
    [JsonProperty("name")]
    public string Name;

    [JsonProperty("displayName")]
    public string DisplayName;

    [JsonProperty("hardness")]
    public float? Hardness;

    [JsonProperty("resistance")]
    public float Resistance;

    [JsonProperty("diggable")]
    public bool Diggable;

    [JsonProperty("transparent")]
    public bool Transparent;

    [JsonProperty("filterLight")]
    public int FilterLight;

    [JsonProperty("emitLight")]
    public int EmitLight;

    [JsonProperty("boundingBox")]
    public string BoundingBox;

    [JsonProperty("stackSize")]
    public int StackSize;

    [JsonProperty("material")]
    public string? Material;

    [JsonProperty("defaultState")]
    public int? DefaultState;

    [JsonProperty("minStateId")]
    public int? MinStateId;

    [JsonProperty("maxStateId")]
    public int? MaxStateId;

    [JsonProperty("harvestTools")]
    public Dictionary<string, bool>? HarvestTools;

    [JsonProperty("states")]
    public BlockStateBlob[]? States;

    [JsonProperty("drops")]
    public object[] Drops;
}

internal class BlockStateBlob
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

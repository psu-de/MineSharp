using MineSharp.Core.Common.Biomes;
using Newtonsoft.Json;

namespace MineSharp.Data.Json;

internal class BiomeInfoBlob : IDataBlob<BiomeInfo>
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("category")]
    public string Category { get; set; }

    [JsonProperty("temperature")]
    public float Temperature { get; set; }

    [JsonProperty("precipitation")]
    public string Precipitation { get; set; }

    [JsonProperty("depth")]
    public float Depth { get; set; }

    [JsonProperty("dimension")]
    public string Dimension { get; set; }

    [JsonProperty("displayName")]
    public string DisplayName { get; set; }

    [JsonProperty("color")]
    public int Color { get; set; }

    [JsonProperty("rainfall")]
    public float Rainfall { get; set; }


    public BiomeInfo ToElement()
    {
        return new BiomeInfo(
            this.Id,
            this.Name,
            this.Category,
            this.Temperature,
            this.Precipitation,
            this.Depth,
            this.Dimension,
            this.DisplayName,
            this.Color,
            this.Rainfall);
    }
}

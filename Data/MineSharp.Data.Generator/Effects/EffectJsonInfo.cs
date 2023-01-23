using Newtonsoft.Json;

namespace MineSharp.Data.Generator.Effects
{
#pragma warning disable CS8618
    internal class EffectJsonInfo
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

        [JsonProperty("type")]
        [Index(3)]
        public string Type;
    }
#pragma warning restore CS8618
}

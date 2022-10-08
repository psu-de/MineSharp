using Newtonsoft.Json;

namespace MineSharp.Data.Generator.Effects
{
#pragma warning disable CS8618
    internal class EffectJsonInfo
    {

        [JsonProperty("id")]
        public int Id;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("displayName")]
        public string DisplayName;
        [JsonProperty("type")]
        public string Type;

    }
#pragma warning restore CS8618
}

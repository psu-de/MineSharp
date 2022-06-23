using Newtonsoft.Json;

namespace MineSharp.Data.Effects {
#pragma warning disable CS8618
    public class EffectJsonInfo {

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

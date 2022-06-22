using Newtonsoft.Json;

namespace MineSharp.Data.Effects {
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
}

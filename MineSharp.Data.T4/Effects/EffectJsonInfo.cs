using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.T4.Effects {
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

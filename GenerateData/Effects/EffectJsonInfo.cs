using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateData.Effects {
    internal class EffectJsonInfo {

        [JsonProperty("id")]
        public int Id;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("displayName")]
        public string DislayName;
        [JsonProperty("type")]
        public string Type;

    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateData.Blocks {
    internal class BlockShapeJsonInfo {

        [JsonProperty("blocks")]
        public Dictionary<string, object> Blocks { get; set; }
        [JsonProperty("shapes")]

        public Dictionary<string, List<List<float>>> Shapes { get; set; }

    }
}

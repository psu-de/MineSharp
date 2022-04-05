﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateData.Items {
    public class ItemJsonInfo {

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("stackSize")]
        public int StackSize { get; set; }

        [JsonProperty("maxDurability")]
        public int? MaxDurability { get; set; }

        [JsonProperty("enchantCategories")]
        public List<string>? EnchantCategories { get; set; }

        [JsonProperty("repairWith")]
        public List<string>? RepairWith { get; set; }

    }
}

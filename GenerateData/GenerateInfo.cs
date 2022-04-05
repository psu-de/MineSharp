using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateData {
    public struct GenerateInfo {
        [JsonProperty("attributes")]
        public string AttributesPath;
        [JsonProperty("blocks")]
        public string BlocksPath;
        [JsonProperty("blockCollisionShapes")]
        public string BlockCollisionShapesPath;
        [JsonProperty("biomes")]
        public string BiomesPath;
        [JsonProperty("enchantments")]
        public string EnchantmentsPath;
        [JsonProperty("effects")]
        public string EffectsPath;
        [JsonProperty("items")]
        public string ItemsPath;
        [JsonProperty("recipes")]
        public string RecipesPath;
        [JsonProperty("instruments")]
        public string InstrumentsPath;
        [JsonProperty("materials")]
        public string MaterialsPath;
        [JsonProperty("entities")]
        public string EntitiesPath;
        [JsonProperty("protocol")]
        public string ProtocolPath;
        [JsonProperty("windows")]
        public string WindowsPath;
        [JsonProperty("version")]
        public string VersionPath;
        [JsonProperty("language")]
        public string LanguagePath;
        [JsonProperty("foods")]
        public string FoodsPath;
        [JsonProperty("particles")]
        public string ParticlesPath;
        [JsonProperty("tints")]
        public string TintsPath;
        [JsonProperty("mapIcons")]
        public string MapIcons;
    }

    public struct DataPaths {
        [JsonProperty("pc")]
        public Dictionary<string, GenerateInfo> PCVersions;
        [JsonProperty("bedrock")]
        public Dictionary<string, object> BedrockVersions;
    }
}

using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace MineSharp.Data.Generator {
    internal class MinecraftDataHelper {


        public string DataPath;

        private DataPathsJson DataPaths;

        public MinecraftDataHelper(string dataPath) {
            this.DataPath = Path.Join(dataPath, "data");

            this.DataPaths = JsonConvert.DeserializeObject<DataPathsJson>(File.ReadAllText(Path.Join(DataPath, "dataPaths.json")));
        }


        public string[] GetAvailableVersions() => DataPaths.PCVersions.Keys.ToArray();

        internal string GetJsonPath(string version, string type) {

            var pathInfo = DataPaths.PCVersions[version];
            var key = Uppercase(type) + "Path";
            string dataPath = (string)(pathInfo.GetType().GetProperty(key, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)?.GetValue(DataPaths.PCVersions[version]) ?? throw new KeyNotFoundException(key));
            return Path.Join(DataPath, dataPath, type + ".json");

        }

        public T LoadJson<T>(string version, string type) {
            var data = JsonConvert.DeserializeObject<T>(File.ReadAllText(GetJsonPath(version, type)));
            if (data == null)
                throw new Exception();
            return data;
        }

        public string GetCSharpName(string name) {
            System.Globalization.TextInfo ti = new System.Globalization.CultureInfo("en-US", false).TextInfo;
            name = name.Replace("_", " ");
            name = ti.ToTitleCase(name);

            Regex rgx = new Regex(@"^\d+");
            Match match = rgx.Match(name);
            if (match.Success) {
                name = name.Substring(match.Value.Length);
                name += match.Value;
            }

            rgx = new Regex("[^a-zA-Z0-9 -]");
            name = rgx.Replace(name, "");
            name = name.Replace(" ", "");
            return Uppercase(name);
        }

        public string Uppercase(string str) {
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        public string Lowercase(string str) {
            return char.ToLower(str[0]) + str.Substring(1);
        }

#pragma warning disable CS8618
        private struct GenerateInfoJson {
            [JsonProperty("attributes")]
            public string AttributesPath { get; set; }
            [JsonProperty("blocks")]
            public string BlocksPath { get; set; }
            [JsonProperty("blockCollisionShapes")]
            public string BlockCollisionShapesPath { get; set; }
            [JsonProperty("biomes")]
            public string BiomesPath { get; set; }
            [JsonProperty("enchantments")]
            public string EnchantmentsPath { get; set; }
            [JsonProperty("effects")]
            public string EffectsPath { get; set; }
            [JsonProperty("items")]
            public string ItemsPath { get; set; }
            [JsonProperty("recipes")]
            public string RecipesPath { get; set; }
            [JsonProperty("instruments")]
            public string InstrumentsPath { get; set; }
            [JsonProperty("materials")]
            public string MaterialsPath { get; set; }
            [JsonProperty("entities")]
            public string EntitiesPath { get; set; }
            [JsonProperty("protocol")]
            public string ProtocolPath { get; set; }
            [JsonProperty("windows")]
            public string WindowsPath { get; set; }
            [JsonProperty("version")]
            public string VersionPath { get; set; }
            [JsonProperty("language")]
            public string LanguagePath { get; set; }
            [JsonProperty("foods")]
            public string FoodsPath { get; set; }
            [JsonProperty("particles")]
            public string ParticlesPath { get; set; }
            [JsonProperty("tints")]
            public string TintsPath { get; set; }
            [JsonProperty("mapIcons")]
            public string MapIcons { get; set; }
        }

        private struct DataPathsJson {
#pragma warning disable CS0649
            [JsonProperty("pc")]
            public Dictionary<string, GenerateInfoJson> PCVersions;
            [JsonProperty("bedrock")]
            public Dictionary<string, object> BedrockVersions;
#pragma warning restore CS0649
        }
    }
#pragma warning restore CS8618
}

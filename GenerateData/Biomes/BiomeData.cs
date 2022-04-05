using GenerateData.Blocks;
using GenerateData.Generators;
using Newtonsoft.Json;
using System.Globalization;

namespace GenerateData.Biomes {
    public class BiomeData {

        public static void Generate(string biomeJsonData, string outDir) {

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            List<BiomeJsonInfo>? Biomes = JsonConvert.DeserializeObject<List<BiomeJsonInfo>>(File.ReadAllText(biomeJsonData));

            if (Biomes == null) {
                throw new Exception("Could not load biomes!");
            }

            List<string> BiomeCategories = new List<string>();

            EnumGenerator BiomeTypesEnum = new EnumGenerator();
            EnumGenerator BiomeCategoryEnum = new EnumGenerator();
            ClassGenerator biomeData = new ClassGenerator();

            Directory.CreateDirectory(Path.Join(outDir, "Biomes"));

            int lastId = 0;
            foreach (var biomeInfo in Biomes) {
                if (!BiomeCategories.Contains(biomeInfo.category)) {
                    BiomeCategories.Add(biomeInfo.category);
                    BiomeCategoryEnum.EnumAddValue(Generator.Uppercase(Generator.MakeCSharpSafe(biomeInfo.category)));
                }
                if (biomeInfo.id != lastId++) throw new Exception("LastId should be current id-1");
                string staticName = Generator.MakeCSharpSafe(biomeInfo.displayName.Replace(" ", ""));
                BiomeTypesEnum.EnumAddValue(staticName, biomeInfo.id);
                string biomeInfoStr = $"new BiomeInfo({biomeInfo.id}, \"{biomeInfo.name}\", BiomeType.{Generator.Uppercase(staticName)}, BiomeCategory.{Generator.Uppercase(Generator.MakeCSharpSafe(biomeInfo.category))}, {biomeInfo.temperature.ToString(nfi)}f, \"{biomeInfo.precipitation}\", {biomeInfo.depth.ToString(nfi)}f, Dimension.{Generator.Uppercase(biomeInfo.dimension)}, \"{biomeInfo.displayName}\", {biomeInfo.color}, {biomeInfo.rainfall.ToString(nfi)}f)";
                biomeData.AddLoaderExpression($"RegisterBiome({biomeInfoStr});");
            }


            biomeData.WithRegisterFunction(@"private static void RegisterBiome(BiomeInfo info){ 
                Biomes.Add(info);
        }");

            biomeData.AddClassVariable("public static List<BiomeInfo> Biomes = new List<BiomeInfo>();")
                     .AddClassVariable("public static Dictionary<BiomeType, BiomeInfo> BiomeMap = new Dictionary<BiomeType, BiomeInfo>();")
                     .WithName("BiomeData")
                     .WithNamespace("Data.Biomes")
                     .Write(Path.Join(outDir, "Biomes", "BiomeData.cs"));
            BiomeTypesEnum.WithNamespace("Data.Biomes").WithName("BiomeType").Write(Path.Join(outDir, "Biomes", "BiomeType.cs"));
            BiomeCategoryEnum.WithNamespace("Data.Biomes").WithName("BiomeCategory").Write(Path.Join(outDir, "Biomes", "BiomeCategory.cs"));
        }
    }
}

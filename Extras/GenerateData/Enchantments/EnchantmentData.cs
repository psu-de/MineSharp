using GenerateData.Generators;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateData.Enchantments {
    public static class EnchantmentData {

        public static void Generate(string dataPath, string outDir) {

            var data = JsonConvert.DeserializeObject<EnchantmentJsonInfo[]>(File.ReadAllText(dataPath));

            EnumGenerator enumGen = new EnumGenerator();
            EnumGenerator enumCatGen = new EnumGenerator();
            ClassGenerator classGen = new ClassGenerator();

            List<string> categories = new List<string>();
            foreach (var item in data) {

                var name = Generator.MakeCSharpSafe(item.DisplayName.Replace(" ", ""));
                var catType = Generator.MakeCSharpSafe(item.Category[0].ToString().ToUpper() + item.Category.Substring(1)).Replace(" ", "");
                if (!categories.Contains(catType)) {
                    categories.Add(catType);
                    enumCatGen.EnumAddValue(catType);
                }
                enumGen.EnumAddValue(name, item.Id);
                classGen.AddLoaderExpression($"Register(EnchantmentType.{name}, \"{item.Name}\", \"{item.DisplayName}\", {item.MaxLevel}, new EnchantCost({item.MinCost.A}, {item.MinCost.B}), new EnchantCost({item.MaxCost.A}, {item.MaxCost.B}), {item.TreasureOnly.ToString().ToLower()}, {item.Curse.ToString().ToLower()}, new string[] {{ {string.Join(", ", item.Exclude.Select(x => $"\"{x}\"")) } }}, EnchantmentCategory.{catType}, {item.Weight}, {item.Discoverable.ToString().ToLower()});");
            }

            Directory.CreateDirectory(Path.Join(outDir, "Enchantments"));

            classGen.AddClassVariable(@"public static Dictionary<EnchantmentType, EnchantmentInfo> Enchantments = new Dictionary<EnchantmentType, EnchantmentInfo>();");
            classGen.WithRegisterFunction(@"private static void Register(EnchantmentType type, string name, string displayName, int maxLevel, EnchantCost minCost, EnchantCost maxCost, bool treasureOnly, bool curse, string[] exclude, EnchantmentCategory category, int weight, bool discoverable) {
            EnchantmentInfo info = new EnchantmentInfo(type, name, displayName, maxLevel, minCost, maxCost, treasureOnly, curse, exclude, category, weight, discoverable);
            Enchantments.Add(type, info);
            }");

            classGen.WithName("EnchantmentData")
                .WithNamespace("Data.Enchantments")
                .Write(Path.Join(outDir, "Enchantments", "EnchantmentData.cs"));

            enumGen.WithName("EnchantmentType")
                .WithNamespace("Data.Enchantments")
                .Write(Path.Join(outDir, "Enchantments", "EnchantmentType.cs"));
            enumCatGen.WithName("EnchantmentCategory")
                .WithNamespace("Data.Enchantments")
                .Write(Path.Join(outDir, "Enchantments", "EnchantmentCategory.cs"));
        }

    }
}

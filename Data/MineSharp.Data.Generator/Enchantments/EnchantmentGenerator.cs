using MineSharp.Core.Types;
using System.Globalization;

namespace MineSharp.Data.Generator.Enchantments
{
    internal class EnchantmentGenerator : Generator
    {
        public EnchantmentGenerator(MinecraftDataHelper wrapper, string version) : base(wrapper, version) {}

        public string[] GetUsings()
        {
            return new[] {
                "MineSharp.Core.Types", "System.Collections.Generic"
            };
        }

        public override void WriteCode(CodeGenerator codeGenerator)
        {

            var enchantmentData = this.Wrapper.LoadJson<EnchantmentJsonInfo[]>(this.Version, "enchantments");

            var nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            codeGenerator.CommentBlock($"Generated Enchantment Data for Minecraft Version {this.Version}");

            foreach (var ns in this.GetUsings())
                codeGenerator.WriteLine($"using {ns};");



            var infoGeneratorTemplate = new InfoGeneratorTemplate<EnchantmentJsonInfo>() {
                Name = "Enchantment",
                Data = enchantmentData,
                Namespace = "MineSharp.Data.Enchantments",
                NameGenerator = (t) => this.Wrapper.GetCSharpName(t.Name),
                Indexer = (t) => t.Id,
                Stringifiers = new Dictionary<string, Func<object, string>>() {
                    { "Id", (t) => InfoGenerator<int>.StringifyDefaults(t) },
                    { "Name", (t) => InfoGenerator<int>.StringifyDefaults(t) },
                    { "DisplayName", (t) => InfoGenerator<int>.StringifyDefaults(t) },
                    { "MaxLevel", (t) => InfoGenerator<int>.StringifyDefaults(t) },
                    { "MinCost", (t) => $@"new EnchantCost({(t as MinCost).A}, {(t as MinCost).B})" },
                    { "MaxCost", (t) => $@"new EnchantCost({(t as MaxCost).A}, {(t as MaxCost).B})" },
                    { "TreasureOnly", (t) => InfoGenerator<int>.StringifyDefaults(t) },
                    { "Curse", (t) => InfoGenerator<int>.StringifyDefaults(t) },
                    { "Exclude", (t) => InfoGenerator<int>.StringifyArray<int>((t as List<string>).Select(x => enchantmentData.First(u => u.Name == x)).Select(x => x.Id).ToArray(), (i) => InfoGenerator<int>.StringifyDefaults(i)) },
                    { "Category", (t) => $"(int)EnchantmentCategoryType.{this.Wrapper.GetCSharpName(t as string)}" },
                    { "Weight", (t) => InfoGenerator<int>.StringifyDefaults(t) },
                    { "Tradeable", (t) => InfoGenerator<int>.StringifyDefaults(t) },
                    { "Discoverable", (t) => InfoGenerator<int>.StringifyDefaults(t) },
                },
                Indexers = new EnumGenerator<EnchantmentJsonInfo>[]
                {
                    new EnumGenerator<EnchantmentJsonInfo>()
                    {
                        GetName = (e) => this.Wrapper.GetCSharpName(e.Category),
                        Name = "EnchantmentCategory"
                    }
                }
            };
            var infoGenerator = new InfoGenerator<EnchantmentJsonInfo>(infoGeneratorTemplate);
            infoGenerator.GenerateInfos(codeGenerator);


//            codeGenerator.Begin("namespace MineSharp.Data.Enchantments");

//            codeGenerator.Begin("public static class EnchantmentPalette");

//            codeGenerator.Begin("public static Type GetEnchantmentTypeById(int id) => id switch");
//            foreach (var enchantment in enchantmentData)
//                codeGenerator.WriteLine($"{enchantment.Id} => typeof({this.Wrapper.GetCSharpName(enchantment.Name)}),");
//            codeGenerator.WriteLine("_ => throw new ArgumentException($\"Enchantment with id {id} not found!\")");
//            codeGenerator.Finish(semicolon: true);
//            codeGenerator.Finish();

//            codeGenerator.Begin("public enum EnchantmentCategory");
//            var categories = enchantmentData.Select(x => $"{this.Wrapper.GetCSharpName(x.Category)}").Distinct().ToList();
//            foreach (var category in categories)
//                codeGenerator.WriteLine($"{category} = {categories.IndexOf(category)},");
//            codeGenerator.Finish();

//            foreach (var enchantment in enchantmentData)
//            {

//                codeGenerator.Begin($"public class {this.Wrapper.GetCSharpName(enchantment.Name)} : Enchantment");
//                codeGenerator.WriteBlock($@"
//public const int EnchantmentId = {enchantment.Id};
//public const string EnchantmentName = ""{enchantment.Name}"";
//public const string EnchantmentDisplayName = ""{enchantment.DisplayName}"";

//public const int EnchantmentMaxLevel = {enchantment.MaxLevel};
//public static readonly EnchantCost EnchantmentMinCost = new EnchantCost({enchantment.MinCost.A}, {enchantment.MinCost.B});
//public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost({enchantment.MaxCost.A}, {enchantment.MaxCost.B});
//public const bool EnchantmentTreasureOnly = {enchantment.TreasureOnly.ToString().ToLower()};
//public const bool EnchantmentCurse = {enchantment.Curse.ToString().ToLower()};
//public static readonly Type[] EnchantmentExclude = new Type[] {{ {string.Join(", ", enchantment.Exclude.Select(x => $"typeof({this.Wrapper.GetCSharpName(enchantment.Name)})"))} }};
//public const int EnchantmentCategory = {categories.IndexOf(this.Wrapper.GetCSharpName(enchantment.Category))};
//public const int EnchantmentWeight = {enchantment.Weight};
//public const bool EnchantmentDiscoverable = {enchantment.Discoverable.ToString().ToLower()};


//public {this.Wrapper.GetCSharpName(enchantment.Name)} () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {{}}

//public {this.Wrapper.GetCSharpName(enchantment.Name)} (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {{}}");
//                codeGenerator.Finish();

//            }

//            codeGenerator.Begin("public enum EnchantmentType");
//            foreach (var enchantment in enchantmentData)
//                codeGenerator.WriteLine($"{this.Wrapper.GetCSharpName(enchantment.Name)} = {enchantment.Id},");
//            codeGenerator.Finish();
//            codeGenerator.Finish();
        }
    }
}

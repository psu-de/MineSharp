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

            codeGenerator.Begin("namespace MineSharp.Data.Enchantments");

            codeGenerator.Begin("public static class EnchantmentPalette");

            codeGenerator.Begin("public static Type GetEnchantmentTypeById(int id) => id switch");
            foreach (var enchantment in enchantmentData)
                codeGenerator.WriteLine($"{enchantment.Id} => typeof({this.Wrapper.GetCSharpName(enchantment.Name)}),");
            codeGenerator.WriteLine($"_ => throw new ArgumentException($\"Enchantment with id {{id}} not found!\")");
            codeGenerator.Finish(semicolon: true);
            codeGenerator.Finish();

            codeGenerator.Begin("public enum EnchantmentCategory");
            List<string>? categories = enchantmentData.Select(x => $"{this.Wrapper.GetCSharpName(x.Category)}").Distinct().ToList();
            foreach (var category in categories)
                codeGenerator.WriteLine($"{category} = {categories.IndexOf(category)},");
            codeGenerator.Finish();

            foreach (var enchantment in enchantmentData)
            {

                codeGenerator.Begin($"public class {this.Wrapper.GetCSharpName(enchantment.Name)} : Enchantment");
                codeGenerator.WriteBlock($@"
public const int EnchantmentId = {enchantment.Id};
public const string EnchantmentName = ""{enchantment.Name}"";
public const string EnchantmentDisplayName = ""{enchantment.DisplayName}"";

public const int EnchantmentMaxLevel = {enchantment.MaxLevel};
public static readonly EnchantCost EnchantmentMinCost = new EnchantCost({enchantment.MinCost.A}, {enchantment.MinCost.B});
public static readonly EnchantCost EnchantmentMaxCost = new EnchantCost({enchantment.MaxCost.A}, {enchantment.MaxCost.B});
public const bool EnchantmentTreasureOnly = {enchantment.TreasureOnly.ToString().ToLower()};
public const bool EnchantmentCurse = {enchantment.Curse.ToString().ToLower()};
public static readonly Type[] EnchantmentExclude = new Type[] {{ {string.Join(", ", enchantment.Exclude.Select(x => $"typeof({this.Wrapper.GetCSharpName(enchantment.Name)})"))} }};
public const int EnchantmentCategory = {categories.IndexOf(this.Wrapper.GetCSharpName(enchantment.Category))};
public const int EnchantmentWeight = {enchantment.Weight};
public const bool EnchantmentDiscoverable = {enchantment.Discoverable.ToString().ToLower()};


public {this.Wrapper.GetCSharpName(enchantment.Name)} () : base(EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {{}}

public {this.Wrapper.GetCSharpName(enchantment.Name)} (int level) : base(level, EnchantmentId, EnchantmentName, EnchantmentDisplayName, EnchantmentMaxLevel, EnchantmentMinCost, EnchantmentMaxCost, EnchantmentTreasureOnly, EnchantmentCurse, EnchantmentExclude, EnchantmentCategory, EnchantmentWeight, EnchantmentDiscoverable) {{}}");
                codeGenerator.Finish();

            }

            codeGenerator.Begin("public enum EnchantmentType");
            foreach (var enchantment in enchantmentData)
                codeGenerator.WriteLine($"{this.Wrapper.GetCSharpName(enchantment.Name)} = {enchantment.Id},");
            codeGenerator.Finish();
            codeGenerator.Finish();
        }
    }
}

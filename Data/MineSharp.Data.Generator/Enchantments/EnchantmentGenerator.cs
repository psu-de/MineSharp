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
        }
    }
}

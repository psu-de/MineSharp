using System.Globalization;
using System.Text;

namespace MineSharp.Data.Generator.Biomes
{
    internal class BiomeGenerator : Generator
    {
        public BiomeGenerator(MinecraftDataHelper wrapper, string version) : base(wrapper, version) {}

        public string[] GetUsings()
        {
            return new[] {
                "MineSharp.Core.Types"
            };
        }

        public override void WriteCode(CodeGenerator codeGenerator)
        {

            var nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            var biomeData = this.Wrapper.LoadJson<BiomeJsonInfo[]>(this.Version, "biomes");

            codeGenerator.CommentBlock($"Generated Biome Data for Minecraft Version {this.Version}");

            var categoryEnum = new EnumGenerator<BiomeJsonInfo>()
            {
                GetName = (b) => this.Wrapper.GetCSharpName(b.Category),
                Name = "BiomeCategory",
            };

            foreach (var ns in this.GetUsings())
                codeGenerator.WriteLine($"using {ns};");

                var infoGeneratorTemplate = new InfoGeneratorTemplate<BiomeJsonInfo>() {
                Name = "Biome",
                Namespace = "MineSharp.Data.Biomes",
                Stringifiers = new Dictionary<string, Func<object, string>>() {
                    { "Id", (x) => InfoGenerator<int>.StringifyDefaults(x) },
                    { "Name", (x) => InfoGenerator<int>.StringifyDefaults(x) },
                    { "Category", (x) => $"(int)BiomeCategoryType.{this.Wrapper.GetCSharpName(x as string)}" },
                    { "Temperature", (x) => InfoGenerator<int>.StringifyDefaults(x) },
                    { "Precipitation", (x) => InfoGenerator<int>.StringifyDefaults(x) },
                    { "Depth", (x) => InfoGenerator<int>.StringifyDefaults(x) },
                    { "Dimension", (x) => $"MineSharp.Core.Types.Enums.Dimension.{this.Wrapper.Uppercase(x as string)}" },
                    { "DisplayName", (x) => InfoGenerator<int>.StringifyDefaults(x) },
                    { "Color", (x) => InfoGenerator<int>.StringifyDefaults(x) },
                    { "Rainfall", (x) => InfoGenerator<int>.StringifyDefaults(x) },
                },
                Data = biomeData,
                NameGenerator = (d) => this.Wrapper.GetCSharpName(d.Name),
                Indexer = (x) => x.Id,
                Indexers = new EnumGenerator<BiomeJsonInfo>[]
                {
                    categoryEnum
                }
            };
            var infoGenerator = new InfoGenerator<BiomeJsonInfo>(infoGeneratorTemplate);
            infoGenerator.GenerateInfos(codeGenerator);
        }

        private string GetName(string name) => this.Wrapper.GetCSharpName(name);
    }
}

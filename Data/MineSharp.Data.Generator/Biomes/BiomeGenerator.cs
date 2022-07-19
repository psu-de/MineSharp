using MineSharp.Core.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Generator.Biomes {
    internal class BiomeGenerator : Generator {

        public BiomeGenerator(MinecraftDataHelper wrapper, string version) : base(wrapper, version) { }


        public string[] GetUsings() {
            return new[] { "MineSharp.Core.Types" }; 
        }

        public override void WriteCode(CodeGenerator codeGenerator) {

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            var biomeData = Wrapper.LoadJson<BiomeJsonInfo[]>(Version, "biomes");

            codeGenerator.CommentBlock($"Generated Biome Data for Minecraft Version {Version}");
            
            foreach (var ns in GetUsings())
                codeGenerator.WriteLine($"using {ns};");

            codeGenerator.Begin("namespace MineSharp.Data.Biomes");

            codeGenerator.Begin($"public static class BiomePalette");
            codeGenerator.Begin($"public static Type GetBiomeTypeById(int id) => id switch");
            foreach (var biome in biomeData)
                codeGenerator.WriteLine($"{biome.Id} => typeof({GetName(biome.Name)}),");
            codeGenerator.WriteLine($@"_ => throw new ArgumentException($""Biome with id {{id}} not found!"")");
            codeGenerator.Finish(semicolon: true);
            codeGenerator.Finish();

            codeGenerator.Begin("public enum BiomeCategory");
            var categories = biomeData.Select(x => $"{Wrapper.GetCSharpName(x.Category)}").Distinct().ToList();
            foreach (var biomeCategory in categories)
                codeGenerator.WriteLine($"{biomeCategory} = {categories.IndexOf(biomeCategory)},");
            codeGenerator.Finish();

            foreach (var biome in biomeData) {
                codeGenerator.Begin($"public class {GetName(biome.Name)} : Biome");
                codeGenerator.WriteBlock($@"
public const int BiomeId = {biome.Id};
public const string BiomeName = ""{biome.Name}"";
public const string BiomeDisplayName = ""{biome.DisplayName}"";
public const int BiomeCategory = {categories.IndexOf(Wrapper.GetCSharpName(biome.Category))};
public const float BiomeTemperature = {biome.Temperature.ToString(nfi)}F;
public const string BiomePrecipitation = ""{biome.Precipitation}"";
public const float BiomeDepth = {biome.Depth.ToString(nfi)}F;
public const MineSharp.Core.Types.Enums.Dimension BiomeDimension = MineSharp.Core.Types.Enums.Dimension.{Wrapper.Uppercase(biome.Dimension)};
public const int BiomeColor = {biome.Color};
public const float BiomeRainfall = {biome.Rainfall.ToString(nfi)}F;

public {GetName(biome.Name)}() : base(BiomeId, BiomeName, BiomeDisplayName, BiomeCategory, BiomeTemperature, BiomePrecipitation, BiomeDepth, BiomeDimension, BiomeColor, BiomeRainfall) {{ }}");
                codeGenerator.Finish();
            }

            codeGenerator.Begin("public enum BiomeType");
            foreach (var biome in biomeData)
                codeGenerator.WriteLine($"{GetName(biome.Name)} = {biome.Id},");
            codeGenerator.Finish();

            codeGenerator.Finish();
        }

        private string GetName(string name) => Wrapper.GetCSharpName(name);
    }
}

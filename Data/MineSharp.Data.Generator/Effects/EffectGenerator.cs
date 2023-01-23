using MineSharp.Core.Types;
using MineSharp.Data.Generator.Blocks;
using System.Globalization;

namespace MineSharp.Data.Generator.Effects
{
    internal class EffectGenerator : Generator
    {
        public EffectGenerator(MinecraftDataHelper wrapper, string version) : base(wrapper, version) {}

        public string[] GetUsings()
        {
            return new[] {
                "MineSharp.Core.Types", "System.Collections.Generic"
            };
        }

        public override void WriteCode(CodeGenerator codeGenerator)
        {

            var effectData = this.Wrapper.LoadJson<EffectJsonInfo[]>(this.Version, "effects");

            var nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            codeGenerator.CommentBlock($"Generated Effect Data for Minecraft Version {this.Version}");
            foreach (var ns in this.GetUsings())
                codeGenerator.WriteLine($"using {ns};");

            var infoGeneratorTemplate = new InfoGeneratorTemplate<EffectJsonInfo>() {
                Data = effectData,
                Namespace = "MineSharp.Data.Effects",
                Name = "Effect",
                Indexer = (e) => e.Id,
                NameGenerator = (e) => $"{ this.Wrapper.GetCSharpName(e.Name) }Effect",
                Stringifiers = new Dictionary<string, Func<object, string>>() {
                    { "Id", (e) => InfoGenerator<int>.StringifyDefaults(e) },
                    { "Name", (e) => InfoGenerator<int>.StringifyDefaults(e) },
                    { "DisplayName", (e) => InfoGenerator<int>.StringifyDefaults(e) },
                    { "Type", (e) => InfoGenerator<int>.StringifyDefaults((e as string)! == "good") },
                },
            };

            var infoGenerator = new InfoGenerator<EffectJsonInfo>(infoGeneratorTemplate);
            infoGenerator.GenerateInfos(codeGenerator);
        }
    }
}

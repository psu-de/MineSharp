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

//            codeGenerator.Begin("namespace MineSharp.Data.Effects");

//            codeGenerator.Begin("public static class EffectPalette");

//            codeGenerator.Begin("public static Type GetEffectTypeById(int id) => id switch");
//            foreach (var effect in effectData)
//                codeGenerator.WriteLine($"{effect.Id} => typeof({this.Wrapper.GetCSharpName(effect.Name)}Effect),");
//            codeGenerator.WriteLine("_ => throw new ArgumentException($\"Effect with id {id} not found!\")");
//            codeGenerator.Finish(semicolon: true);
//            codeGenerator.Finish();

//            foreach (var effect in effectData)
//            {

//                codeGenerator.Begin($"public class {this.Wrapper.GetCSharpName(effect.Name)}Effect : Effect");
//                codeGenerator.WriteBlock($@"
//public const int EffectId = {effect.Id};
//		public const string EffectName = ""{effect.Name}"";
//		public const string EffectDisplayName = ""{effect.DisplayName}"";
//        public const bool EffectIsGood = {(effect.Type == "good").ToString().ToLower()};


//        public {this.Wrapper.GetCSharpName(effect.Name)}Effect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {{}} 
//		public {this.Wrapper.GetCSharpName(effect.Name)}Effect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {{}}");
//                codeGenerator.Finish();

//            }

//            codeGenerator.Begin("public enum EffectType");
//            foreach (var effect in effectData)
//                codeGenerator.WriteLine($"{this.Wrapper.GetCSharpName(effect.Name)} = {effect.Id},");
//            codeGenerator.Finish();
//            codeGenerator.Finish();
        }
    }
}

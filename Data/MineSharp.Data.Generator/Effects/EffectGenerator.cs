using System.Globalization;

namespace MineSharp.Data.Generator.Effects
{
    internal class EffectGenerator : Generator {
        public EffectGenerator(MinecraftDataHelper wrapper, string version) : base(wrapper, version) {
        }

        public string[] GetUsings() {
            return new[] { "MineSharp.Core.Types", "System.Collections.Generic" };
        }

        public override void WriteCode(CodeGenerator codeGenerator) {

            var effectData = Wrapper.LoadJson<EffectJsonInfo[]>(Version, "effects");

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            codeGenerator.CommentBlock($"Generated Effect Data for Minecraft Version {Version}");
            foreach (var ns in GetUsings())
                codeGenerator.WriteLine($"using {ns};");

            codeGenerator.Begin("namespace MineSharp.Data.Effects");

            codeGenerator.Begin("public static class EffectPalette");

            codeGenerator.Begin("public static Type GetEffectTypeById(int id) => id switch");
            foreach (var effect in effectData)
                codeGenerator.WriteLine($"{effect.Id} => typeof({Wrapper.GetCSharpName(effect.Name)}Effect),");
            codeGenerator.WriteLine($"_ => throw new ArgumentException($\"Effect with id {{id}} not found!\")");
            codeGenerator.Finish(semicolon: true);
            codeGenerator.Finish();

            foreach (var effect in effectData) {

                codeGenerator.Begin($"public class {Wrapper.GetCSharpName(effect.Name)}Effect : Effect");
                codeGenerator.WriteBlock($@"
public const int EffectId = {effect.Id};
		public const string EffectName = ""{effect.Name}"";
		public const string EffectDisplayName = ""{effect.DisplayName}"";
        public const bool EffectIsGood = {(effect.Type == "good").ToString().ToLower()};


        public {Wrapper.GetCSharpName(effect.Name)}Effect() : base(EffectId, EffectName, EffectDisplayName, EffectIsGood) {{}} 
		public {Wrapper.GetCSharpName(effect.Name)}Effect(int amplifier, DateTime startTime, int duration) : base(amplifier, startTime, duration, EffectId, EffectName, EffectDisplayName, EffectIsGood) {{}}");
                codeGenerator.Finish();

            }

            codeGenerator.Begin("public enum EffectType");
            foreach (var effect in effectData)
                codeGenerator.WriteLine($"{Wrapper.GetCSharpName(effect.Name)} = {effect.Id},");
            codeGenerator.Finish();
            codeGenerator.Finish();
        }
    }
}

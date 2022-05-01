using GenerateData.Generators;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateData.Effects {
    internal class EffectData {

        public static void Generate(string effectPath, string outDir) {

            List<EffectJsonInfo> effectData = JsonConvert.DeserializeObject<List<EffectJsonInfo>>(File.ReadAllText(effectPath));
            if (effectData == null) {
                Console.WriteLine("Could not load effect data");
                return;
            }

            EnumGenerator effectEnum = new EnumGenerator();
            ClassGenerator dataGen = new ClassGenerator();

            foreach (var effect in effectData) {
                string effectEnumName = Generator.MakeCSharpSafe(effect.DislayName.Replace(" ", ""));
                effectEnum.EnumAddValue(effectEnumName, effect.Id);
                dataGen.AddLoaderExpression($"Register(\"{effect.Name}\", \"{effect.DislayName}\", EffectType.{effectEnumName}, {(effect.Type == "good").ToString().ToLower()});");
            }

            dataGen.AddClassVariable(@"public static Dictionary<EffectType, EffectInfo> Effects = new Dictionary<EffectType, EffectInfo>();");
            dataGen.WithRegisterFunction(@"private static void Register(string name, string displayName, EffectType type, bool isGood){ 
            EffectInfo info = new EffectInfo(name, displayName, type, isGood);
            Effects.Add(type, info);
        }");

            Directory.CreateDirectory(Path.Join(outDir, "Effects"));

            effectEnum.WithName("EffectType")
                .WithNamespace("Data.Effects")
                .Write(Path.Join(outDir, "Effects", "EffectType.cs"));
            dataGen.WithName("EffectData")
                .WithNamespace("Data.Effects")
                .Write(Path.Join(outDir, "Effects", "EffectData.cs"));
        }

    }
}

using Humanizer;
using MineSharp.SourceGenerator.Code;
using MineSharp.SourceGenerator.Generators.Core;
using MineSharp.SourceGenerator.Utils;
using Newtonsoft.Json.Linq;

namespace MineSharp.SourceGenerator.Generators;

public class EffectGenerator : CommonGenerator
{
    protected override string DataKey => "effects";
    protected override string Namespace => "Effects";
    protected override string Singular => "Effect";

    protected override JToken[] GetProperties(JToken data) 
        => ((JArray)data).ToArray();
    
    protected override string GetName(JToken token)
        => NameUtils.GetEffectName((string)token.SelectToken("name")!);
    
    protected override string Stringify(JToken token)
    {
        var id = (int)token.SelectToken("id")!;
        var name = (string)token.SelectToken("name")!;
        var displayName = (string)token.SelectToken("displayName")!;
        var isGood = (string)token.SelectToken("type")! == "good";
        
        return $"new EffectInfo({id}, " +
               $"EffectType.{name.Pascalize()}, " +
               $"{Str.String(name)}, " +
               $"{Str.String(displayName)}, " +
               $"{Str.Bool(isGood)})";
    }
}

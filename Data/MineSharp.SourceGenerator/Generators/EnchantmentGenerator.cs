using Humanizer;
using MineSharp.SourceGenerator.Code;
using MineSharp.SourceGenerator.Generators.Core;
using MineSharp.SourceGenerator.Utils;
using Newtonsoft.Json.Linq;

namespace MineSharp.SourceGenerator.Generators;

public class EnchantmentGenerator : CommonGenerator
{
    protected override string DataKey => "enchantments";
    protected override string Namespace => "Enchantments";
    protected override string Singular => "Enchantment";
    protected override string[] ExtraUsings { get; } = { "MineSharp.Core.Common" };
    
    protected override async Task WriteAdditionalItems(MinecraftDataWrapper wrapper)
    {
        var outdir = DirectoryUtils.GetCoreSourceDirectory("Common\\Enchantments");
        var enchantments = await wrapper.GetEnchantments(Config.LatestVersion);
        var enchantmentCategories = new HashSet<string>();

        foreach (var enchantment in (JArray)enchantments)
        {
            enchantmentCategories.Add(((string)enchantment.SelectToken("category")!).Pascalize());
        }

        await new EnumGenerator() {
            Namespace = "MineSharp.Core.Common.Enchantments",
            ClassName = "EnchantmentCategory",
            Outfile = Path.Join(outdir, "EnchantmentCategory.cs"),
            Entries = enchantmentCategories
                .Select((x, i) => (x, i))
                .ToDictionary(x => x.x, x => x.i)
        }.Write();
    }

    protected override JToken[] GetProperties(JToken data) 
        => ((JArray)data).ToArray();

    protected override string GetName(JToken token)
        => NameUtils.GetEnchantmentName((string)token.SelectToken("name")!);
    
    protected override string Stringify(JToken token)
    {
        var id = (int)token.SelectToken("id")!;
        var name = (string)token.SelectToken("name")!;
        var displayName = (string)token.SelectToken("displayName")!;
        var maxLevel = (int)token.SelectToken("maxLevel")!;
        var minCost = GetEnchantCost(token.SelectToken("minCost")!);
        var maxCost = GetEnchantCost(token.SelectToken("maxCost")!);
        var treasureOnly = (bool)token.SelectToken("treasureOnly")!;
        var curse = (bool)token.SelectToken("curse")!;
        var exclude = GetExclusions(token.SelectToken("exclude")!);
        var category = (string)token.SelectToken("category")!;
        var weight = (int)token.SelectToken("weight")!;
        var tradeable = (bool)token.SelectToken("tradeable")!;
        var discoverable = (bool)token.SelectToken("discoverable")!;

        return $"new EnchantmentInfo({id}, " +
               $"EnchantmentType.{name.Pascalize()}, " +
               $"{Str.String(name)}, " +
               $"{Str.String(displayName)}, " +
               $"{maxLevel}, " +
               $"{minCost}, " +
               $"{maxCost}, " +
               $"{Str.Bool(treasureOnly)}, " +
               $"{Str.Bool(curse)}, " +
               $"{exclude}, " +
               $"EnchantmentCategory.{category.Pascalize()}, " +
               $"{weight}, " +
               $"{Str.Bool(tradeable)}, " +
               $"{Str.Bool(discoverable)})";
    }

    private string GetEnchantCost(JToken token)
    {
        var a = (int)token.SelectToken("a")!;
        var b = (int)token.SelectToken("b")!;
        return $"new EnchantCost({a}, {b})";
    }

    private string GetExclusions(JToken token)
    {
        var list = new List<string>();
        foreach (var val in (JArray)token)
        {
            list.Add(((string)val!).Pascalize());
        }
        
        if (list.Count == 0)
            return "Array.Empty<EnchantmentType>()";

        return $"new [] {{ {string.Join(", ", list.Select(x => $"EnchantmentType.{x}"))} }}";
    }
}

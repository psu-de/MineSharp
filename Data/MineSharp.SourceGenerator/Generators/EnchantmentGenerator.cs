using Humanizer;
using MineSharp.SourceGenerator.Code;
using MineSharp.SourceGenerator.Utils;
using Newtonsoft.Json.Linq;

namespace MineSharp.SourceGenerator.Generators;

public class EnchantmentGenerator : IGenerator
{
    public string Name => "Enchantment";
    
    public async Task Run(MinecraftDataWrapper wrapper)
    {
        await GenerateEnum(wrapper);
        
        foreach (var version in Config.IncludedVersions)
        {
            await GenerateVersion(wrapper, version);
        }
    }
    
    private async Task GenerateEnum(MinecraftDataWrapper wrapper)
    {
        var outdir = DirectoryUtils.GetCoreSourceDirectory("Common\\Enchantments");
        var enchantments = await wrapper.GetEnchantments(Config.LatestVersion);

        var enchantmentValues = new Dictionary<string, int>();
        var enchantmentCategories = new HashSet<string>();

        foreach (var enchantment in (JArray)enchantments)
        {
            enchantmentValues.Add(((string)enchantment.SelectToken("name")!).Pascalize(), (int)enchantment.SelectToken("id")!);
            enchantmentCategories.Add(((string)enchantment.SelectToken("category")!).Pascalize());
        }

        await new EnumGenerator() {
            Namespace = "MineSharp.Core.Common.Enchantments",
            ClassName = "EnchantmentType",
            Outfile = Path.Join(outdir, "EnchantmentType.cs"),
            Entries = enchantmentValues
        }.Write();

        await new EnumGenerator() {
            Namespace = "MineSharp.Core.Common.Enchantments",
            ClassName = "EnchantmentCategory",
            Outfile = Path.Join(outdir, "EnchantmentCategory.cs"),
            Entries = enchantmentCategories
                .Select((x, i) => (x, i))
                .ToDictionary(x => x.x, x => x.i)
        }.Write();
    }
    
    private async Task GenerateVersion(MinecraftDataWrapper wrapper, string version)
    {
        var path = wrapper.GetPath(version, "enchantments");
        if (VersionMapGenerator.GetInstance().IsRegistered("enchantments", path))
        {
            VersionMapGenerator.GetInstance().RegisterVersion("enchantments", version, path);
            return;
        }
        
        VersionMapGenerator.GetInstance().RegisterVersion("enchantments", version, path);
        
        var outdir = DirectoryUtils.GetDataSourceDirectory("Enchantments\\Versions");
        var v = path.Replace("pc/", "").Replace(".", "_");
        var enchantments = await wrapper.GetEnchantments(version);

        await new DataVersionGenerator() {
            Namespace = "MineSharp.Data.Enchantments.Versions",
            ClassName = $"Enchantments_{v}",
            EnumName = "EnchantmentType",
            InfoClass = "EnchantmentInfo",
            Usings = new[] { "MineSharp.Core.Common", "MineSharp.Core.Common.Enchantments" },
            Outfile = Path.Join(outdir, $"Enchantments_{v}.cs"),
            Properties = ((JArray)enchantments).ToArray(),
            Stringify = Stringify,
            KeySelector = KeySelector
        }.Write();
    }

    private string KeySelector(JToken token)
    {
        return ((string)token.SelectToken("name")!).Pascalize();
    }
    
    private string Stringify(JToken token)
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

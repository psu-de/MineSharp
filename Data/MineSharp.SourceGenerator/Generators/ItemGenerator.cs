using Humanizer;
using MineSharp.SourceGenerator.Code;
using MineSharp.SourceGenerator.Utils;
using Newtonsoft.Json.Linq;

namespace MineSharp.SourceGenerator.Generators;

public class ItemGenerator : IGenerator
{
    public string Name => "Item";

    public async Task Run(MinecraftDataWrapper wrapper)
    {
        await GenerateEnum(wrapper);
        
        foreach (var version in Config.IncludedVersions)
        {
            await GenerateVersion(wrapper, version);
        }
    }
    
    private async Task GenerateVersion(MinecraftDataWrapper wrapper, string version)
    {
        var path = wrapper.GetPath(version, "items");
        if (VersionMapGenerator.GetInstance().IsRegistered("items", path))
        {
            VersionMapGenerator.GetInstance().RegisterVersion("items", version, path);
            return;
        }
        
        VersionMapGenerator.GetInstance().RegisterVersion("items", version, path);
        
        var outdir = DirectoryUtils.GetDataSourceDirectory("Items\\Versions");
        var v = path.Replace("pc/", "").Replace(".", "_");
        var items = await wrapper.GetItems(version);

        await new DataVersionGenerator() {
            Namespace = "MineSharp.Data.Items.Versions",
            ClassName = $"Items_{v}",
            EnumName = "ItemType",
            InfoClass = "ItemInfo",
            Usings = new[] { "MineSharp.Core.Common.Enchantments", "MineSharp.Core.Common.Items" },
            Outfile = Path.Join(outdir, $"Items_{v}.cs"),
            Properties = ((JArray)items).ToArray(),
            Stringify = Stringify,
            KeySelector = KeySelector
        }.Write();
    }

    private async Task GenerateEnum(MinecraftDataWrapper wrapper)
    {
        var outdir = DirectoryUtils.GetCoreSourceDirectory("Common\\Items");
        var items = await wrapper.GetItems(Config.LatestVersion);

        var itemValues = new Dictionary<string, int>();

        foreach (var item in (JArray)items)
        {
            itemValues.Add(((string)item.SelectToken("name")!).Pascalize(), (int)item.SelectToken("id")!);
        }

        await new EnumGenerator() {
            Namespace = "MineSharp.Core.Common.Items",
            ClassName = "ItemType",
            Outfile = Path.Join(outdir, "ItemType.cs"),
            Entries = itemValues
        }.Write();
    }
    
    private string KeySelector(JToken token)
    {
        var name = (string)token.SelectToken("name")!;
        return NameUtils.GetItemName(name);
    }
    
    private string Stringify(JToken token)
    {
        var id = (int)token.SelectToken("id")!;
        var name = (string)token.SelectToken("name")!;
        var displayName = (string)token.SelectToken("displayName")!;
        var stackSize = (int)token.SelectToken("stackSize")!;
        var maxDurability = ((int?)token.SelectToken("maxDurability"))?.ToString() ?? "null";
        var enchantCategories = this.GetEnchantmentCategories(token.SelectToken("enchantCategories"));
        var repairWith = GetRepairWith(token.SelectToken("repairWith"));

        return $"new ItemInfo({id}, " +
               $"ItemType.{NameUtils.GetItemName(name)}, " +
               $"{Str.String(name)}, " +
               $"{Str.String(displayName)}, " +
               $"{stackSize}, " +
               $"{maxDurability}, " +
               $"{enchantCategories}, " +
               $"{repairWith})";
    }

    private string GetEnchantmentCategories(JToken? token)
    {
        if (null == token)
            return "null";
        
        var list = new List<string>();
        foreach (var item in (JArray)token)
        {
            list.Add(((string)item!).Pascalize());
        }
        
        if (list.Count == 0)
            return "Array.Empty<EnchantmentCategory>()";

        return $"new [] {{ {string.Join(", ", list.Select(x => $"EnchantmentCategory.{x}"))} }}";
    }

    private string GetRepairWith(JToken? token)
    {
        if (null == token)
            return "null";
        
        var list = new List<string>();
        foreach (var item in (JArray)token)
        {
            list.Add(((string)item!).Pascalize());
        }

        if (list.Count == 0)
            return $"Array.Empty<ItemType>()";

        return $"new [] {{ {string.Join(", ", list.Select(x => $"ItemType.{x}"))} }}";
    }
}

using Humanizer;
using MineSharp.SourceGenerator.Code;
using MineSharp.SourceGenerator.Generators.Core;
using MineSharp.SourceGenerator.Utils;
using Newtonsoft.Json.Linq;

namespace MineSharp.SourceGenerator.Generators;

public class BlockGenerator : CommonGenerator
{
    protected override string Singular => "Block";
    protected override string Namespace => "Blocks";
    protected override string DataKey => "blocks";
    protected override string[] ExtraUsings { get; } = 
        { "MineSharp.Core.Common.Blocks.Property", "MineSharp.Core.Common.Items" };

    private JToken? _currentVersionItems;

    protected override string GetName(JToken token)
        => NameUtils.GetBlockName((string)token.SelectToken("name")!);

    protected override JToken[] GetProperties(JToken data) 
        => ((JArray)data).ToArray();

    protected override async Task GenerateVersion(MinecraftDataWrapper wrapper, string version)
    {
        this._currentVersionItems = await wrapper.GetItems(version);
        await base.GenerateVersion(wrapper, version);
    }

    protected override async Task WriteAdditionalItems(MinecraftDataWrapper wrapper)
    {
        var outdir = DirectoryUtils.GetCoreSourceDirectory("Common\\Blocks");
        var blocks = await wrapper.GetBlocks(Config.LatestVersion);

        var materials = new HashSet<string>();
        foreach (var block in (JArray)blocks)
        {
            var mats = (string)block.SelectToken("material")!;
            foreach (var material in mats.Split(";"))
            {
                materials.Add(NameUtils.GetMaterial(material));
            }
        }

        await new EnumGenerator() {
            Namespace = "MineSharp.Core.Common.Blocks",
            ClassName = "Material",
            Outfile = Path.Join(outdir, "Material.cs"),
            Entries = materials.Select((x, i) => (x, i)).ToDictionary(x => x.x, x => x.i)
        }.Write();
    }

    protected override string Stringify(JToken token)
    {
        var id = (int)token.SelectToken("id")!;
        var name = (string)token.SelectToken("name")!;
        var displayName = (string)token.SelectToken("displayName")!;
        var hardness = (float?)token.SelectToken("hardness")! ?? -1;
        var resistance = (float)token.SelectToken("resistance")!;
        var minId = (int)token.SelectToken("minStateId")!;
        var maxId = (int)token.SelectToken("maxStateId")!;
        var diggable = (bool)token.SelectToken("diggable")!;
        var transparent = (bool)token.SelectToken("transparent")!;
        var filterLight = (int)token.SelectToken("filterLight")!;
        var emitLight = (int)token.SelectToken("emitLight")!;
        var bbox = (string)token.SelectToken("boundingBox")!;
        var stackSize = (int)token.SelectToken("stackSize")!;
        var materials = GetMaterials(token.SelectToken("material")!);
        var harvestTools = GetHarvestTools(token.SelectToken("harvestTools"));
        var defaultState = (int)token.SelectToken("defaultState")!;
        var states = GetBlockState(token.SelectToken("states")!);

        return $"new BlockInfo({id}, " +
               $"BlockType.{name.Pascalize()}, " +
               $"{Str.String(name)}, " +
               $"{Str.String(displayName)}, " +
               $"{Str.Float(hardness)}, " +
               $"{Str.Float(resistance)}, " +
               $"{minId}, " +
               $"{maxId}, " +
               $"{Str.Bool(diggable)}, " +
               $"{Str.Bool(transparent)}, " +
               $"{filterLight}, " +
               $"{emitLight}, " +
               $"{Str.String(bbox)}, " +
               $"{stackSize}, " +
               $"{materials}, " +
               $"{harvestTools}, " +
               $"{defaultState}, " +
               $"{states})";
    }

    private string GetBlockState(JToken token)
    {
        var list = new List<string>();
        foreach (var state in (JArray)token)
        {
            list.Add(StringifyState(state));
        }
        return $"new BlockState({string.Join(", ", list)})";
    }

    private string StringifyState(JToken state)
    {
        var name = (string)state.SelectToken("name")!;
        var type = (string)state.SelectToken("type")!;
        var numValues = (int)state.SelectToken("num_values")!;

        switch (type)
        {
            case "bool":
                return $"new BoolProperty({Str.String(name)})";
            case "int":
                return $"new IntProperty({Str.String(name)}, {numValues})";
            case "enum":
                var values = ((JArray)state.SelectToken("values")!)
                    .Select(x => Str.String((string)x!))
                    .ToArray();
                return $"new EnumProperty({Str.String(name)}, new [] {{ {string.Join(", ", values)} }})";
            default:
                throw new Exception("Not found: " + type);
        }
    }

    private string GetHarvestTools(JToken? token)
    {
        if (token == null)
            return "null";
        
        string FindNameFromItemId(string id)
        {
            foreach (var token in this._currentVersionItems!)
            {
                var itemId = (int)token.SelectToken("id")!;
                if (itemId.ToString() == id)
                    return NameUtils.GetItemName((string)token.SelectToken("name")!);
            }
            throw new Exception($"Item not found: {id}");
        }


        var arr = ((JObject)token).Properties()
            .Select(x => x.Name)
            .Select(FindNameFromItemId)
            .Select(x => $"ItemType.{x}")
            .ToArray();

        if (arr.Length == 0)
            return "null";

        return $"new [] {{ {string.Join(", ", arr)} }}";
    }
    
    private string GetMaterials(JToken token)
    {
        var str = (string)token!;

        var mats = str.Split(";")
            .Select(NameUtils.GetMaterial)
            .Select(x => $"Material.{x}")
            .ToArray();
        return mats.Length == 0
            ? "Array.Empty<Material>()"
            : $"new [] {{ {string.Join(", ", mats)} }}";
    }
}

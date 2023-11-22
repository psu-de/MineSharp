using Humanizer;
using MineSharp.SourceGenerator.Code;
using MineSharp.SourceGenerator.Utils;
using Newtonsoft.Json.Linq;

namespace MineSharp.SourceGenerator.Generators;

public class BlockGenerator : IGenerator
{
    public string Name => "Block";
    
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
        var path = wrapper.GetPath(version, "blocks");
        if (VersionMapGenerator.GetInstance().IsRegistered("blocks", path))
        {
            VersionMapGenerator.GetInstance().RegisterVersion("blocks", version, path);
            return;
        }
        
        VersionMapGenerator.GetInstance().RegisterVersion("blocks", version, path);
        
        var outdir = DirectoryUtils.GetDataSourceDirectory("Blocks\\Versions");
        var v = path.Replace("pc/", "").Replace(".", "_");
        var blocks = await wrapper.GetBlocks(version);

        await new DataVersionGenerator() {
            Namespace = "MineSharp.Data.Blocks.Versions",
            ClassName = $"Blocks_{v}",
            EnumName = "BlockType",
            InfoClass = "BlockInfo",
            Usings = new[] { "MineSharp.Core.Common.Blocks", "MineSharp.Core.Common.Blocks.Property" },
            Outfile = Path.Join(outdir, $"Blocks_{v}.cs"),
            Properties = ((JArray)blocks).ToArray(),
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

        var arr = ((JObject)token).Properties()
            .Select(x => x.Name)
            .Select(x => Convert.ToInt32(x))
            .ToArray();

        return $"new int[] {{ {string.Join(", ", arr)} }}";
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
    
    private async Task GenerateEnum(MinecraftDataWrapper wrapper)
    {
        var outdir = DirectoryUtils.GetCoreSourceDirectory("Common\\Blocks");
        var blocks = await wrapper.GetBlocks(Config.LatestVersion);

        var blockValues = new Dictionary<string, int>();
        var materials = new HashSet<string>();
        foreach (var block in (JArray)blocks)
        {
            blockValues.Add(((string)block.SelectToken("name")!).Pascalize(), (int)block.SelectToken("id")!);
            var mats = (string)block.SelectToken("material")!;
            foreach (var material in mats.Split(";"))
            {
                materials.Add(NameUtils.GetMaterial(material));
            }
        }

        await new EnumGenerator() {
            Namespace = "MineSharp.Core.Common.Blocks",
            ClassName = "BlockType",
            Outfile = Path.Join(outdir, "BlockType.cs"),
            Entries = blockValues
        }.Write();

        await new EnumGenerator() {
            Namespace = "MineSharp.Core.Common.Blocks",
            ClassName = "Material",
            Outfile = Path.Join(outdir, "Material.cs"),
            Entries = materials.Select((x, i) => (x, i)).ToDictionary(x => x.x, x => x.i)
        }.Write();
    }
}

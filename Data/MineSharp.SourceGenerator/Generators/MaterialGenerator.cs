using MineSharp.SourceGenerator.Code;
using MineSharp.SourceGenerator.Utils;
using Newtonsoft.Json.Linq;
using System.Text;

namespace MineSharp.SourceGenerator.Generators;

public class MaterialGenerator : IGenerator
{
    public string Name => "Materials";

    public async Task Run(MinecraftDataWrapper wrapper)
    {
        foreach (var version in Config.IncludedVersions)
        {
            await GenerateVersion(version, wrapper);
        }
    }

    private async Task GenerateVersion(string version, MinecraftDataWrapper wrapper)
    {        
        var path = wrapper.GetPath(version, "materials");
        if (VersionMapGenerator.GetInstance().IsRegistered("materials", path))
        {
            VersionMapGenerator.GetInstance().RegisterVersion("materials", version, path);
            return;
        }
        
        VersionMapGenerator.GetInstance().RegisterVersion("materials", version, path);

        var outdir = DirectoryUtils.GetDataSourceDirectory("Materials\\Versions");
        var materials = await wrapper.GetMaterials(version);
        var items = (JArray)await wrapper.GetItems(version)!;

        string FindNameFromItemId(string id)
        {
            foreach (var token in items)
            {
                var itemId = (int)token.SelectToken("id")!;
                if (itemId.ToString() == id)
                    return NameUtils.GetItemName((string)token.SelectToken("name")!);
            }
            throw new Exception($"Item not found: {id}");
        }
        
        var v = version.Replace(".", "_");
        
        var writer = new CodeWriter();
        writer.Disclaimer();
        writer.WriteLine("using MineSharp.Core.Common.Blocks;");
        writer.WriteLine("using MineSharp.Core.Common.Items;");
        writer.WriteLine();
        writer.WriteLine("namespace MineSharp.Data.Materials.Versions;");
        writer.WriteLine();
        writer.Begin($"internal class Materials_{v} : MaterialVersion");
        writer.Begin("public override IDictionary<Material, Dictionary<ItemType, float>> Palette { get; } = new()");
        foreach (var prop in ((JObject)materials).Properties())
        {
            if (prop.Name.Contains(';'))
                continue;
            
            writer.Begin("");
            writer.WriteLine($"Material.{NameUtils.GetMaterial(prop.Name)},");
            writer.Begin($"new Dictionary<ItemType, float>()");
            foreach (var kvp in ((JObject)prop.Value).Properties())
            {
                writer.WriteLine($"{{ ItemType.{FindNameFromItemId(kvp.Name)}, {Str.Float((float)kvp.Value)} }},");
            }
            writer.Finish();
            writer.Finish(colon: true);
        }
        writer.Finish(semicolon: true);
        writer.Finish();
        
        await File.WriteAllTextAsync(Path.Join(outdir, $"Materials_{v}.cs"), writer.ToString());
    }
}

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
        
        var sb = new StringBuilder();
        sb.AppendLine("using MineSharp.Core.Common.Blocks;");
        sb.AppendLine("using MineSharp.Core.Common.Items;");
        sb.AppendLine();
        sb.AppendLine("namespace MineSharp.Data.Materials.Versions;");
        sb.AppendLine();
        sb.AppendLine($"internal class Materials_{v} : MaterialVersion");
        sb.AppendLine("{");
        sb.AppendLine("    public override IDictionary<Material, Dictionary<ItemType, float>> Palette { get; } = new Dictionary<Material, Dictionary<ItemType, float>>()");
        sb.AppendLine("    {");
        foreach (var prop in ((JObject)materials).Properties())
        {
            if (prop.Name.Contains(';'))
                continue;
            
            sb.AppendLine("        {");
            sb.AppendLine($"            Material.{NameUtils.GetMaterial(prop.Name)},");
            sb.AppendLine($"            new Dictionary<ItemType, float>()");
            sb.AppendLine($"            {{");
            foreach (var kvp in ((JObject)prop.Value).Properties())
            {
                sb.AppendLine($"                {{ ItemType.{FindNameFromItemId(kvp.Name)}, {Str.Float((float)kvp.Value)} }},");
            }
            sb.AppendLine($"            }}");
            sb.AppendLine("        },");
        }
        sb.AppendLine("    };");
        sb.AppendLine("}");
        
        await File.WriteAllTextAsync(Path.Join(outdir, $"Materials_{v}.cs"), sb.ToString());
    }
}

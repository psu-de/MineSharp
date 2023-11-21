using Humanizer;
using MineSharp.SourceGenerator.Code;
using MineSharp.SourceGenerator.Utils;
using Newtonsoft.Json.Linq;
using System.Text;

namespace MineSharp.SourceGenerator.Generators;

public class BlockCollisionShapesGenerator : IGenerator
{
    public string Name => "BlockCollisionShapes";

    public async Task Run(MinecraftDataWrapper wrapper)
    {
        foreach (var version in Config.IncludedVersions)
        {
            await GenerateVersion(wrapper, version);
        }
    }
    
    private async Task GenerateVersion(MinecraftDataWrapper wrapper, string version)
    {
        var path = wrapper.GetPath(version, "blockCollisionShapes");
        if (VersionMapGenerator.GetInstance().IsRegistered("blockCollisionShapes", path))
        {
            VersionMapGenerator.GetInstance().RegisterVersion("blockCollisionShapes", version, path);
            return;
        }
        
        VersionMapGenerator.GetInstance().RegisterVersion("blockCollisionShapes", version, path);
        
        var outdir = DirectoryUtils.GetDataSourceDirectory("BlockCollisionShapes\\Versions");
        var v = path.Replace("pc/", "").Replace(".", "_");
        var blockCollisionShapes = await wrapper.GetBlockCollisionShapes(version);

        var blocks = (JObject)blockCollisionShapes.SelectToken("blocks")!;
        var shapes = (JObject)blockCollisionShapes.SelectToken("shapes")!;

        var sb = new StringBuilder();
        sb.AppendLine("using MineSharp.Core.Common;");
        sb.AppendLine("using MineSharp.Core.Common.Blocks;");
        sb.AppendLine();
        sb.AppendLine("namespace MineSharp.Data.BlockCollisionShapes.Versions;");
        sb.AppendLine();
        sb.AppendLine($"internal class BlockCollisionShapes_{v} : BlockCollisionShapesVersion");
        sb.AppendLine("{");
        sb.AppendLine("    public override Dictionary<BlockType, int[]> BlockToIndicesMap { get; } = new Dictionary<BlockType, int[]>()");
        sb.AppendLine("    {");
        foreach (var prop in blocks.Properties())
        {
            sb.AppendLine($"        {{ BlockType.{prop.Name.Pascalize()}, {StringifyIndices(prop.Value)} }},");
        }
        sb.AppendLine("    };");
        sb.AppendLine("    public override Dictionary<int, AABB[]> BlockShapes { get; } = new Dictionary<int, AABB[]>()");
        sb.AppendLine("    {");
        foreach (var prop in shapes.Properties())
        {
            sb.AppendLine($"        {{ {prop.Name}, {StringifyShapes(prop.Value)} }},");
        }
        sb.AppendLine("    };");
        sb.AppendLine("}");

        await File.WriteAllTextAsync(
            Path.Join(outdir, $"BlockCollisionShapes_{v}.cs"), sb.ToString());
    }

    private string StringifyIndices(JToken value)
    {
        if (value.Type == JTokenType.Integer)
            return $"new [] {{ {(int)value} }}";

        var values = ((JArray)value).Select(x => (int)x).ToArray();
        return $"new [] {{ {string.Join(", ", values)} }}";
    }
    
    private string StringifyShapes(JToken value) 
    {
        var values = ((JArray)value).Select(x => $"new AABB({string.Join(", ", ((JArray)x).Select(y => Str.Float((float)y)))})")
            .ToArray();

        if (values.Length == 0)
            return "Array.Empty<AABB>()";
        
        return $"new [] {{ {string.Join(", ", values)} }}";
    }
}

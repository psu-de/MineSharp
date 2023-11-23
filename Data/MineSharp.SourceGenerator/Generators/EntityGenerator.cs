using Humanizer;
using MineSharp.SourceGenerator.Code;
using MineSharp.SourceGenerator.Utils;
using Newtonsoft.Json.Linq;

namespace MineSharp.SourceGenerator.Generators;

public class EntityGenerator : IGenerator
{
    public string Name => "Entity";

    public async Task Run(MinecraftDataWrapper wrapper)
    {
        await GenerateEnum(wrapper);

        foreach (var version in Config.IncludedVersions)
        {
            await GenerateVersion(wrapper, version);
        }
    }

    private string GetCategory(JToken token)
    {
        var val = (string)token!;
        if (val == "UNKNOWN")
            val = val.ToLower();
        return val.Pascalize();
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
        var width = (float)token.SelectToken("width")!;
        var height = (float)token.SelectToken("height")!;
        var type = (string)token.SelectToken("type")!;
        var category = GetCategory(token.SelectToken("category")!);

        return $"new EntityInfo({id}, " +
               $"EntityType.{name.Pascalize()}, " +
               $"{Str.String(name)}, " +
               $"{Str.String(displayName)}, " +
               $"{Str.Float(width)}, " +
               $"{Str.Float(height)}, " +
               $"MobType.{type.Pascalize()}, " +
               $"EntityCategory.{category})";
    }

    private async Task GenerateVersion(MinecraftDataWrapper wrapper, string version)
    {
        var path = wrapper.GetPath(version, "entities");
        if (VersionMapGenerator.GetInstance().IsRegistered("entities", path))
        {
            VersionMapGenerator.GetInstance().RegisterVersion("entities", version, path);
            return;
        }
        
        VersionMapGenerator.GetInstance().RegisterVersion("entities", version, path);
        
        var outdir = DirectoryUtils.GetDataSourceDirectory("Entities\\Versions");
        var v = path.Replace("pc/", "").Replace(".", "_");
        var entities = await wrapper.GetEntities(version);

        await new DataVersionGenerator() {
            Namespace = "MineSharp.Data.Entities.Versions",
            ClassName = $"Entities_{v}",
            EnumName = "EntityType",
            InfoClass = "EntityInfo",
            Usings = new[] { "MineSharp.Core.Common", "MineSharp.Core.Common.Entities" },
            Outfile = Path.Join(outdir, $"Entities_{v}.cs"),
            Properties = ((JArray)entities).ToArray(),
            Stringify = Stringify,
            KeySelector = KeySelector
        }.Write();
    }
    
    private async Task GenerateEnum(MinecraftDataWrapper wrapper)
    {
        var outdir = DirectoryUtils.GetCoreSourceDirectory("Common\\Entities");
        var entities = await wrapper.GetEntities(Config.LatestVersion);

        var entityValues = new Dictionary<string, int>();
        var entityCategories = new HashSet<string>();
        var entityTypes = new HashSet<string>();

        foreach (var entity in (JArray)entities)
        {
            entityValues.Add(((string)entity.SelectToken("name")!).Pascalize(), (int)entity.SelectToken("id")!);
            var category = GetCategory(entity.SelectToken("category")!);
            entityCategories.Add(category);
            entityTypes.Add(((string)entity.SelectToken("type")!).Pascalize());
        }

        await new EnumGenerator() {
            Namespace = "MineSharp.Core.Common.Entities",
            ClassName = "EntityType",
            Outfile = Path.Join(outdir, "EntityType.cs"),
            Entries = entityValues
        }.Write();

        await new EnumGenerator() {
            Namespace = "MineSharp.Core.Common.Entities",
            ClassName = "EntityCategory",
            Outfile = Path.Join(outdir, "EntityCategory.cs"),
            Entries = entityCategories
                .Select((x, i) => (x, i))
                .ToDictionary(x => x.x, x => x.i)
        }.Write();
        
        await new EnumGenerator() {
            Namespace = "MineSharp.Core.Common.Entities",
            ClassName = "MobType",
            Outfile = Path.Join(outdir, "MobType.cs"),
            Entries = entityTypes
                .Select((x, i) => (x, i))
                .ToDictionary(x => x.x, x => x.i)
        }.Write();
    }
}

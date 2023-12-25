using Humanizer;
using MineSharp.SourceGenerator.Code;
using MineSharp.SourceGenerator.Generators.Core;
using MineSharp.SourceGenerator.Utils;
using Newtonsoft.Json.Linq;

namespace MineSharp.SourceGenerator.Generators;

public class EntityGenerator : CommonGenerator
{
    protected override string DataKey => "entities";
    protected override string Namespace => "Entities";
    protected override string Singular => "Entity";
    protected override string[] ExtraUsings { get; } = { "MineSharp.Core.Common" };

    protected override JToken[] GetProperties(JToken data)
        => ((JArray)data).ToArray();
    
    protected override string GetName(JToken token)
        => NameUtils.GetEntityName((string)token.SelectToken("name")!);
    
    protected override string Stringify(JToken token)
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
    
    protected override async Task WriteAdditionalItems(MinecraftDataWrapper wrapper)
    {
        var outdir = DirectoryUtils.GetCoreSourceDirectory(Path.Join("Common", "Entities"));
        var entities = await wrapper.GetEntities(Config.LatestVersion);

        var entityCategories = new HashSet<string>();
        var entityTypes = new HashSet<string>();

        foreach (var entity in (JArray)entities)
        {
            var category = GetCategory(entity.SelectToken("category")!);
            entityCategories.Add(category);
            entityTypes.Add(((string)entity.SelectToken("type")!).Pascalize());
        }
        
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
    
    private string GetCategory(JToken token)
    {
        var val = (string)token!;
        if (val == "UNKNOWN")
            val = val.ToLower();
        return val.Pascalize();
    }
}

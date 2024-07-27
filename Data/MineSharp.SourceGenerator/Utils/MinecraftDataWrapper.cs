using Newtonsoft.Json.Linq;

namespace MineSharp.SourceGenerator.Utils;

public class MinecraftDataWrapper
{
    private readonly JObject dataPaths;
    private readonly string path;

    public MinecraftDataWrapper(string path)
    {
        this.path = Path.Join(path, "data");
        var dataPath = Path.Join(this.path, "dataPaths.json");
        dataPaths = (JObject)JToken.Parse(File.ReadAllText(dataPath)).SelectToken("pc")!;
    }

    public string GetPath(string version, string key)
    {
        return (string)dataPaths.Property(version)!.Value.SelectToken(key)!;
    }

    public async Task<JToken> Parse(string version, string key)
    {
        var rel = GetPath(version, key);
        var path = Path.Join(this.path, rel, $"{key}.json");
        return JToken.Parse(await File.ReadAllTextAsync(path));
    }

    public Task<JToken> GetBiomes(string version)
    {
        return Parse(version, "biomes");
    }

    public Task<JToken> GetBlocks(string version)
    {
        return Parse(version, "blocks");
    }

    public Task<JToken> GetEffects(string version)
    {
        return Parse(version, "effects");
    }

    public Task<JToken> GetEnchantments(string version)
    {
        return Parse(version, "enchantments");
    }

    public Task<JToken> GetEntities(string version)
    {
        return Parse(version, "entities");
    }

    public Task<JToken> GetItems(string version)
    {
        return Parse(version, "items");
    }

    public Task<JToken> GetBlockCollisionShapes(string version)
    {
        return Parse(version, "blockCollisionShapes");
    }

    public Task<JToken> GetProtocol(string version)
    {
        return Parse(version, "protocol");
    }

    public Task<JToken> GetVersion(string version)
    {
        return Parse(version, "version");
    }

    public Task<JToken> GetMaterials(string version)
    {
        return Parse(version, "materials");
    }
}

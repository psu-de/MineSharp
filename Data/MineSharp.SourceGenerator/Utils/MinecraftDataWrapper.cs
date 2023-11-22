using Newtonsoft.Json.Linq;

namespace MineSharp.SourceGenerator.Utils;

public class MinecraftDataWrapper
{
    private readonly string _path;
    private readonly JObject _dataPaths;
    
    public MinecraftDataWrapper(string path)
    {
        this._path = Path.Join(path, "data");
        var dataPath = Path.Join(this._path, "dataPaths.json");
        this._dataPaths = (JObject)JToken.Parse(File.ReadAllText(dataPath)).SelectToken("pc")!;
    }
    
    public string GetPath(string version, string key) 
        => (string)this._dataPaths.Property(version)!.Value.SelectToken(key)!;

    private async Task<JToken> Parse(string version, string key)
    {
        var rel = GetPath(version, key);
        var path = Path.Join(this._path, rel, $"{key}.json");
        return JToken.Parse(await File.ReadAllTextAsync(path));
    }

    public Task<JToken> GetBiomes(string version)
        => this.Parse(version, "biomes");

    public Task<JToken> GetBlocks(string version)
        => this.Parse(version, "blocks");

    public Task<JToken> GetEffects(string version)
        => this.Parse(version, "effects");

    public Task<JToken> GetEnchantments(string version)
        => this.Parse(version, "enchantments");

    public Task<JToken> GetEntities(string version)
        => this.Parse(version, "entities");
    
    public Task<JToken> GetItems(string version)
        => this.Parse(version, "items");

    public Task<JToken> GetBlockCollisionShapes(string version)
        => this.Parse(version, "blockCollisionShapes");

    public Task<JToken> GetProtocol(string version)
        => this.Parse(version, "protocol");

    public Task<JToken> GetVersion(string version)
        => this.Parse(version, "version");

    public Task<JToken> GetMaterials(string version)
        => this.Parse(version, "materials");
}

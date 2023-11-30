using Humanizer;
using MineSharp.SourceGenerator.Code;
using MineSharp.SourceGenerator.Utils;
using System.Text;

namespace MineSharp.SourceGenerator.Generators;

public class VersionMapGenerator : IGenerator
{
    private static VersionMapGenerator? _instance;

    public static VersionMapGenerator GetInstance()
    {
        if (_instance == null)
            _instance = new VersionMapGenerator();

        return _instance;
    }
    
    public string Name => "VersionMap";

    private Dictionary<string, Dictionary<string, string>> _registered;

    private VersionMapGenerator()
    {
        this._registered = new Dictionary<string, Dictionary<string, string>>();
    }
    
    
    public async Task Run(MinecraftDataWrapper wrapper)
    {
        var writer = new CodeWriter();
        writer.Disclaimer();
        writer.WriteLine("namespace MineSharp.Data;");
        writer.WriteLine();
        writer.Begin("internal static class VersionMap");
        
        foreach (var key in this._registered.Keys)
        {
            var versionMap = this._registered[key];
            
            writer.Begin($"public static Dictionary<string, string> {key.Pascalize()} {{ get; }} = new()");
                
            foreach (var version in versionMap.Keys)
            {
                var className = this.GetClassName(key, versionMap[version]);
                writer.WriteLine($"{{ {Str.String(version)}, {Str.String(className)} }},");
            }
            
            writer.Finish(semicolon: true);
        }
        writer.Begin("public static Dictionary<string, MinecraftVersion> Versions { get; } = new()");
        foreach (var version in Config.IncludedVersions)
        {
            writer.WriteLine($"{{ {Str.String(version)}, {await StringifyVersion(version, wrapper)} }},");
        }
        writer.Finish(semicolon: true);
        writer.Finish();

        await File.WriteAllTextAsync(
            Path.Join(DirectoryUtils.GetDataSourceDirectory(), "VersionMap.cs"), writer.ToString());
    }

    public void RegisterVersion(string key, string version, string path)
    {
        path = TrimPath(path);
        if (!this._registered.ContainsKey(key))
            this._registered.Add(key, new Dictionary<string, string>());
        
        var map = this._registered[key];
        map.Add(version, path);
    }

    public bool IsRegistered(string key, string path)
    {
        path = TrimPath(path);
        if (!this._registered.ContainsKey(key))
            return false;

        var map = this._registered[key];
        return map.ContainsValue(path);
    }

    private string TrimPath(string path)
        => path.Substring("pc/".Length);

    private string GetClassName(string key, string version)
        => $"MineSharp.Data.{key.Pascalize()}.Versions.{key.Pascalize()}_{version.Replace(".", "_")}";

    private async Task<string> StringifyVersion(string version, MinecraftDataWrapper wrapper)
    {
        var v = await wrapper.GetVersion(version);
        var protocol = (int)v.SelectToken("version")!;
        var mcVersion = (string)v.SelectToken("minecraftVersion")!;

        return $"new MinecraftVersion({Str.String(mcVersion)}, {protocol})";
    }
}

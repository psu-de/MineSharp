using MineSharp.Data.Exceptions;
using MineSharp.Data.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Providers;

public class FeatureProvider : IDataProvider
{
    private readonly Dictionary<string, MinecraftVersionRange> _features;
    private readonly Dictionary<string, object> _valueFeatures;

    private readonly string _featurePath;
    private readonly string _versionsPath;
    private readonly MinecraftVersion _version;
    
    public bool IsLoaded { get; private set; }

    public FeatureProvider(string featurePath, string versionsPath, MinecraftVersion version)
    {
        this._featurePath = featurePath;
        this._versionsPath = versionsPath;
        this._version = version;
        
        this._features = new Dictionary<string, MinecraftVersionRange>();
        this._valueFeatures = new Dictionary<string, object>();
    }

    public void Load()
    {
        if (IsLoaded)
        {
            return;
        }
        
        this.LoadData();

        IsLoaded = true;
    }

    private void LoadData()
    {
        var latestVersion = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(this._versionsPath))!.Last();
        var array = JArray.Parse(File.ReadAllText(this._featurePath));

        foreach (var token in array)
        {
            var obj = (JObject)token;
            string name = (string)obj.GetValue("name")!;

            if (obj.ContainsKey("values"))
            {
                InsertValueFeature(name, obj, latestVersion);
                continue;
            }
            
            InsertFeature(name, obj, latestVersion);
        }
    }

    // private object JValueToObject(JValue value)
    // {
    //     return value.Type switch {
    //         JTokenType.Integer => value.Va
    //         JTokenType.String => (string)value
    //     };
    // }
    
    private void InsertValueFeature(string name, JObject obj, string latest)
    {
        JArray values = (JArray)obj.GetValue("values")!;
        MinecraftVersionRange range;
        object? value = null;

        foreach (var entry in values)
        {
            var entryO = (JObject)entry;
            if (entryO.TryGetValue("version", out var versionToken))
            {
                var v = GetVersion((string)versionToken!, latest);
                range = new MinecraftVersionRange(v, v);
            }
            else 
                range = GetVersionRange(entryO.GetValue("versions")!, latest);

            if (!range.ContainsVersion(this._version))
                continue;

            value = (entryO.GetValue("value")! as JValue)!.Value;
            break;
        }

        if (value == null)
        {
            throw new MineSharpDataException(
                $"Couldn't find a feature value for feature ${name} (version: {this._version}).");
        }

        if (!this._valueFeatures.TryAdd(name, value))
        {
            this._valueFeatures[name] = value;
        }
    }
    
    private void InsertFeature(string name, JObject obj, string latest)
    {
        MinecraftVersionRange range;
        if (obj.TryGetValue("version", out var versionToken))
        {
            var v = GetVersion((string)versionToken!, latest);
            range = new MinecraftVersionRange(v, v);
        }
        else
        {
            range = GetVersionRange(obj.GetValue("versions")!, latest);
        }
                
        if (!this._features.TryAdd(name, range))
            this._features[name] = range;
    }

    private MinecraftVersionRange GetFeature(string name)
    {
        if (!this._features.TryGetValue(name, out var feature))
        {
            throw new MineSharpDataException($"Feature with name {name} not found.");
        }

        return feature;
    }

    public T GetValue<T>(string name)
    {
        if (!this._valueFeatures.TryGetValue(name, out var value))
        {
            throw new MineSharpDataException($"Value with name {name} not found.");
        }

        return (T)value;
    }

    public bool Supports(string name)
    {
        return this.GetFeature(name).ContainsVersion(this._version);
    }
    
    private MinecraftVersionRange GetVersionRange(JToken versionsToken, string latest)
    {
        string[] versions = ((JArray)versionsToken).Select(x => (string)x!).ToArray();
        
        if (versions.Length != 2)
            throw new MineSharpDataException("Expected array of length 2");

        return new MinecraftVersionRange(
            GetVersion(versions[0], latest),
            GetVersion(versions[1], latest));
    }

    private MinecraftVersion GetVersion(string version, string latest)
    {
        return new MinecraftVersion(version == "latest" ? latest : version);
    }

    private record FeatureValue(object Value, MinecraftVersion Version);
}

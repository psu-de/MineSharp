using MineSharp.Data.Exceptions;
using Newtonsoft.Json;

namespace MineSharp.Data.Providers;

public class LanguageDataProvider : IDataProvider
{
    private readonly string _path;
    private Dictionary<string, string> _rules;

    public bool IsLoaded { get; private set; }

    public LanguageDataProvider(string languagePath)
    {
        this._path = languagePath;
        this._rules = new Dictionary<string, string>();
        this.IsLoaded = false;
    }

    public void Load()
    {
        if (IsLoaded)
        {
            return;
        }
        
        this._rules = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(this._path))!;

        this.IsLoaded = true;
    }

    public string GetRule(string key)
    {
        if (!this._rules.TryGetValue(key, out var rule))
        {
            throw new MineSharpDataException($"Language rule with key '{key}' not found.");
        }

        return rule;
    }
}

using MineSharp.Core.Common.Effects;
using MineSharp.Data.Exceptions;
using MineSharp.Data.Json;
using Newtonsoft.Json;

namespace MineSharp.Data.Providers;

public class EffectDataProvider : IDataProvider
{
    private readonly Dictionary<int, EffectInfo> _byId;
    private readonly Dictionary<string, EffectInfo> _byName;
    private readonly string _path;
    
    public bool IsLoaded { get; private set; }
    
    public EffectDataProvider(string effectPath)
    {
        this._path = effectPath;
        this._byId = new Dictionary<int, EffectInfo>();
        this._byName = new Dictionary<string, EffectInfo>();

        this.IsLoaded = false;
    }
    
    public void Load()
    {
        if (IsLoaded)
        {
            return;
        }
        
        this.LoadData();

        this.IsLoaded = true;
    }

    private void LoadData()
    {
        EffectInfoBlob[] effectData = JsonConvert.DeserializeObject<EffectInfoBlob[]>(File.ReadAllText(this._path))!;

        foreach (var effectBlob in effectData)
        {
            var bi = new EffectInfo(
                effectBlob.Id,
                effectBlob.Name,
                effectBlob.DisplayName,
                effectBlob.Type == "good");

            this._byId.Add(effectBlob.Id, bi);
            this._byName.Add(effectBlob.Name, bi);
        }
    }

    public EffectInfo GetById(int index)
    {
        if (!this._byId.TryGetValue(index, out var bi))
        {
            throw new MineSharpDataException($"The Block with id {index} was not found.");
        }

        return bi;
    }

    public EffectInfo GetByName(string name)
    {
        if (!this._byName.TryGetValue(name, out var bi))
        {
            throw new MineSharpDataException($"The Block with name '{name}' was not found.");
        }

        return bi;
    }
}
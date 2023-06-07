using MineSharp.Core.Common.Biomes;
using MineSharp.Data.Exceptions;
using MineSharp.Data.Json;
using Newtonsoft.Json;

namespace MineSharp.Data.Providers;

public class BiomeDataProvider : IDataProvider {
    
    private readonly Dictionary<int, BiomeInfo> _byId;
    private readonly Dictionary<string, BiomeInfo> _byName;
    private readonly string _path;
    
    public bool IsLoaded { get; private set; }

    public int Count => this._byId.Count;

    public BiomeDataProvider(string biomePath)
    {
        this._path = biomePath;
        this._byId = new Dictionary<int, BiomeInfo>();
        this._byName = new Dictionary<string, BiomeInfo>();

        this.IsLoaded = false;
    }

    public BiomeInfo GetById(int id)
    {
        if (!this._byId.TryGetValue(id, out var bi))
        {
            throw new MineSharpDataException($"The Block with id {id} was not found.");
        }

        return bi;
    }

    public BiomeInfo GetByName(string name)
    {
        if (!this._byName.TryGetValue(name, out var bi))
        {
            throw new MineSharpDataException($"The Block with name '{name}' was not found.");
        }

        return bi;
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
        BiomeInfoBlob[] biomeData = JsonConvert.DeserializeObject<BiomeInfoBlob[]>(File.ReadAllText(this._path))!;

        foreach (var biomeBlob in biomeData)
        {
            var bi = new BiomeInfo(
                biomeBlob.Id,
                biomeBlob.Name,
                biomeBlob.Category,
                biomeBlob.Temperature,
                biomeBlob.Precipitation,
                biomeBlob.Depth,
                biomeBlob.Dimension,
                biomeBlob.DisplayName,
                biomeBlob.Color,
                biomeBlob.Rainfall);

            this._byId.Add(biomeBlob.Id, bi);
            this._byName.Add(biomeBlob.Name, bi);
        }
    }
}
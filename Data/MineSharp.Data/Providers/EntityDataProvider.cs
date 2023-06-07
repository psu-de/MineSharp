using MineSharp.Core.Common.Entities;
using MineSharp.Data.Exceptions;
using MineSharp.Data.Json;
using Newtonsoft.Json;

namespace MineSharp.Data.Providers;

public class EntityDataProvider : IDataProvider
{
    private readonly Dictionary<int, EntityInfo> _byId;
    private readonly Dictionary<string, EntityInfo> _byName;
    private readonly string _path;

    public bool IsLoaded { get; private set; }

    public EntityDataProvider(string entityPath)
    {
        this._path = entityPath;
        this._byId = new Dictionary<int, EntityInfo>();
        this._byName = new Dictionary<string, EntityInfo>();

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
        EntityInfoBlob[] entityData = JsonConvert.DeserializeObject<EntityInfoBlob[]>(File.ReadAllText(this._path))!;

        foreach (var entityBlob in entityData)
        {
            var bi = new EntityInfo(
                entityBlob.Id,
                entityBlob.Name,
                entityBlob.DisplayName,
                entityBlob.Width,
                entityBlob.Height,
                entityBlob.Category
            );

            this._byId.Add(entityBlob.Id, bi);
            this._byName.Add(entityBlob.Name, bi);
        }
    }

    public EntityInfo GetById(int index)
    {
        if (!this._byId.TryGetValue(index, out var bi))
        {
            throw new MineSharpDataException($"The Block with id {index} was not found.");
        }

        return bi;
    }
    
    public EntityInfo GetByName(string name)
    {
        if (!this._byName.TryGetValue(name, out var bi))
        {
            throw new MineSharpDataException($"The Block with name '{name}' was not found.");
        }

        return bi;
    }
}
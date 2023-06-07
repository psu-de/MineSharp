using MineSharp.Core.Common.Items;
using MineSharp.Data.Exceptions;
using MineSharp.Data.Json;
using Newtonsoft.Json;

namespace MineSharp.Data.Providers;

public class ItemDataProvider : IDataProvider
{
    private readonly Dictionary<int, ItemInfo> _byId;
    private readonly Dictionary<string, ItemInfo> _byName;
    private readonly string _path;

    public bool IsLoaded { get; private set; }

    public ItemDataProvider(string itemsPath)
    {
        this._path = itemsPath;
        this._byId = new Dictionary<int, ItemInfo>();
        this._byName = new Dictionary<string, ItemInfo>();

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
        ItemInfoBlob[] itemData = JsonConvert.DeserializeObject<ItemInfoBlob[]>(File.ReadAllText(this._path))!;

        foreach (var itemBlob in itemData)
        {
            var bi = new ItemInfo(
                itemBlob.Id,
                itemBlob.Name,
                itemBlob.DisplayName,
                itemBlob.StackSize,
                itemBlob.MaxDurability,
                itemBlob.EnchantCategories,
                itemBlob.RepairWith
            );

            this._byId.Add(itemBlob.Id, bi);
            this._byName.Add(itemBlob.Name, bi);
        }
    }

    public ItemInfo GetById(int index)
    {
        if (!this._byId.TryGetValue(index, out var bi))
        {
            throw new MineSharpDataException($"The Block with id {index} was not found.");
        }

        return bi;
    }

    public ItemInfo GetByName(string name)
    {
        if (!this._byName.TryGetValue(name, out var bi))
        {
            throw new MineSharpDataException($"The Block with name {name} was not found.");
        }

        return bi;
    }
}

using MineSharp.Core.Common.Enchantments;
using MineSharp.Data.Exceptions;
using MineSharp.Data.Json;
using Newtonsoft.Json;

namespace MineSharp.Data.Providers;

public class EnchantmentDataProvider : IDataProvider
{
    private readonly Dictionary<int, EnchantmentInfo> _byId;
    private readonly Dictionary<string, EnchantmentInfo> _byName;
    private readonly string _path;

    public bool IsLoaded{ get; private set; }
    
    public EnchantmentDataProvider(string enchantmentPath)
    {
        this._path = enchantmentPath;
        this._byId = new Dictionary<int, EnchantmentInfo>();
        this._byName = new Dictionary<string, EnchantmentInfo>();

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
        EnchantmentInfoBlob[] enchantmentData = JsonConvert.DeserializeObject<EnchantmentInfoBlob[]>(File.ReadAllText(this._path))!;

        foreach (var enchantmentBlob in enchantmentData)
        {
            var bi = new EnchantmentInfo(
                enchantmentBlob.Id,
                enchantmentBlob.Name,
                enchantmentBlob.DisplayName,
                enchantmentBlob.MaxLevel,
                new EnchantCost(enchantmentBlob.MinCost.A, enchantmentBlob.MinCost.B),
                new EnchantCost(enchantmentBlob.MaxCost.A, enchantmentBlob.MaxCost.B),
                enchantmentBlob.TreasureOnly,
                enchantmentBlob.Curse,
                enchantmentBlob.Exclude.ToArray(),
                enchantmentBlob.Category,
                enchantmentBlob.Weight,
                enchantmentBlob.Tradeable,
                enchantmentBlob.Discoverable
            );

            this._byId.Add(enchantmentBlob.Id, bi);
            this._byName.Add(enchantmentBlob.Name, bi);
        }
    }

    public EnchantmentInfo GetById(int index)
    {
        if (!this._byId.TryGetValue(index, out var bi))
        {
            throw new MineSharpDataException($"The Block with id {index} was not found.");
        }

        return bi;
    }
    
    public EnchantmentInfo GetByName(string name)
    {
        if (!this._byName.TryGetValue(name, out var bi))
        {
            throw new MineSharpDataException($"The Block with name '{name}' was not found.");
        }

        return bi;
    }
}
using MineSharp.Core.Common.Items;
using System.Diagnostics.CodeAnalysis;

namespace MineSharp.Data.Items;

public class ItemProvider : DataProvider<ItemType, ItemInfo>
{
    private Dictionary<int, ItemInfo> IdToEnchantmentMap { get; }
    private Dictionary<string, ItemInfo> NameToEnchantmentMap { get; }
    
    internal ItemProvider(DataVersion<ItemType, ItemInfo> version) : base(version)
    {
        this.IdToEnchantmentMap = version.Palette.ToDictionary(x => x.Value.Id, x => x.Value);
        this.NameToEnchantmentMap = version.Palette.ToDictionary(x => x.Value.Name, x => x.Value);
    }
    
    public ItemInfo GetById(int id) => this.IdToEnchantmentMap[id];
    
    public bool TryGetById(int id, [NotNullWhen(true)] out ItemInfo? effect)
        => this.IdToEnchantmentMap.TryGetValue(id, out effect);
    
    public ItemInfo GetByName(string name) => this.NameToEnchantmentMap[name];

    public bool TryGetByName(string name, [NotNullWhen(true)] out ItemInfo? effect)
        => this.NameToEnchantmentMap.TryGetValue(name, out effect);
    

    public ItemInfo this[int id] => GetById(id);

    public ItemInfo this[string name] => GetByName(name);
}

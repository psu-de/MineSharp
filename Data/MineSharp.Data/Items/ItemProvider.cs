using MineSharp.Core.Common.Items;
using System.Diagnostics.CodeAnalysis;

namespace MineSharp.Data.Items;

/// <summary>
/// Provides static data about items.
/// </summary>
public class ItemProvider : DataProvider<ItemType, ItemInfo>
{
    private Dictionary<int, ItemInfo> IdToEnchantmentMap { get; }
    private Dictionary<string, ItemInfo> NameToEnchantmentMap { get; }
    
    internal ItemProvider(DataVersion<ItemType, ItemInfo> version) : base(version)
    {
        this.IdToEnchantmentMap = version.Palette.ToDictionary(x => x.Value.Id, x => x.Value);
        this.NameToEnchantmentMap = version.Palette.ToDictionary(x => x.Value.Name, x => x.Value);
    }
    
    /// <summary>
    /// Get an ItemInfo by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ItemInfo GetById(int id) => this.IdToEnchantmentMap[id];
    
    /// <summary>
    /// Try to get an ItemInfo by id. Returns false if not found.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="effect"></param>
    /// <returns></returns>
    public bool TryGetById(int id, [NotNullWhen(true)] out ItemInfo? effect)
        => this.IdToEnchantmentMap.TryGetValue(id, out effect);
    
    /// <summary>
    /// Get an ItemInfo by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public ItemInfo GetByName(string name) => this.NameToEnchantmentMap[name];

    /// <summary>
    /// Try to get an ItemInfo by name. Returns false if not found.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="effect"></param>
    /// <returns></returns>
    public bool TryGetByName(string name, [NotNullWhen(true)] out ItemInfo? effect)
        => this.NameToEnchantmentMap.TryGetValue(name, out effect);
    
    /// <summary>
    /// Get an ItemInfo by id
    /// </summary>
    /// <param name="id"></param>
    public ItemInfo this[int id] => GetById(id);

    /// <summary>
    /// Get an ItemInfo by name
    /// </summary>
    /// <param name="name"></param>
    public ItemInfo this[string name] => GetByName(name);
}

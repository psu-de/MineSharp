using MineSharp.Core.Common.Entities;
using System.Diagnostics.CodeAnalysis;

namespace MineSharp.Data.Entities;

/// <summary>
/// Provides static data about entities.
/// </summary>
public class EntityProvider : DataProvider<EntityType, EntityInfo>
{
    private Dictionary<int, EntityInfo> IdToEnchantmentMap { get; }
    private Dictionary<string, EntityInfo> NameToEnchantmentMap { get; }
    
    internal EntityProvider(DataVersion<EntityType, EntityInfo> version) : base(version)
    {
        this.IdToEnchantmentMap = version.Palette.ToDictionary(x => x.Value.Id, x => x.Value);
        this.NameToEnchantmentMap = version.Palette.ToDictionary(x => x.Value.Name, x => x.Value);
    }
    
    /// <summary>
    /// Get an EntityInfo by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public EntityInfo GetById(int id) => this.IdToEnchantmentMap[id];
    
    /// <summary>
    /// Try to get an EntityInfo by id. Returns false if not found
    /// </summary>
    /// <param name="id"></param>
    /// <param name="effect"></param>
    /// <returns></returns>
    public bool TryGetById(int id, [NotNullWhen(true)] out EntityInfo? effect)
        => this.IdToEnchantmentMap.TryGetValue(id, out effect);
    
    /// <summary>
    /// Get an EntityInfo by name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public EntityInfo GetByName(string name) => this.NameToEnchantmentMap[name];

    /// <summary>
    /// Try to get an EntityInfo by name. Returns false if not found
    /// </summary>
    /// <param name="name"></param>
    /// <param name="effect"></param>
    /// <returns></returns>
    public bool TryGetByName(string name, [NotNullWhen(true)] out EntityInfo? effect)
        => this.NameToEnchantmentMap.TryGetValue(name, out effect);
    
    /// <summary>
    /// Get an EntityInfo by id
    /// </summary>
    /// <param name="id"></param>
    public EntityInfo this[int id] => GetById(id);

    /// <summary>
    /// Get an EntityInfo by name
    /// </summary>
    /// <param name="name"></param>
    public EntityInfo this[string name] => GetByName(name);
}

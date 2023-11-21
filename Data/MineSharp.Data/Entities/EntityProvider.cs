using MineSharp.Core.Common.Entities;
using System.Diagnostics.CodeAnalysis;

namespace MineSharp.Data.Entities;

public class EntityProvider : DataProvider<EntityType, EntityInfo>
{
    private Dictionary<int, EntityInfo> IdToEnchantmentMap { get; }
    private Dictionary<string, EntityInfo> NameToEnchantmentMap { get; }
    
    internal EntityProvider(DataVersion<EntityType, EntityInfo> version) : base(version)
    {
        this.IdToEnchantmentMap = version.Palette.ToDictionary(x => x.Value.Id, x => x.Value);
        this.NameToEnchantmentMap = version.Palette.ToDictionary(x => x.Value.Name, x => x.Value);
    }
    
    public EntityInfo GetById(int id) => this.IdToEnchantmentMap[id];
    
    public bool TryGetById(int id, [NotNullWhen(true)] out EntityInfo? effect)
        => this.IdToEnchantmentMap.TryGetValue(id, out effect);
    
    public EntityInfo GetByName(string name) => this.NameToEnchantmentMap[name];

    public bool TryGetByName(string name, [NotNullWhen(true)] out EntityInfo? effect)
        => this.NameToEnchantmentMap.TryGetValue(name, out effect);
    

    public EntityInfo this[int id] => GetById(id);

    public EntityInfo this[string name] => GetByName(name);
}

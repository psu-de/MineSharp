using MineSharp.Core.Common.Enchantments;
using System.Diagnostics.CodeAnalysis;

namespace MineSharp.Data.Enchantments;

public class EnchantmentProvider : DataProvider<EnchantmentType, EnchantmentInfo>
{
    private Dictionary<int, EnchantmentInfo> IdToEnchantmentMap { get; }
    private Dictionary<string, EnchantmentInfo> NameToEnchantmentMap { get; }
    
    internal EnchantmentProvider(DataVersion<EnchantmentType, EnchantmentInfo> version) : base(version)
    {
        this.IdToEnchantmentMap = version.Palette.ToDictionary(x => x.Value.Id, x => x.Value);
        this.NameToEnchantmentMap = version.Palette.ToDictionary(x => x.Value.Name, x => x.Value);
    }
    
    public EnchantmentInfo GetById(int id) => this.IdToEnchantmentMap[id];
    
    public bool TryGetById(int id, [NotNullWhen(true)] out EnchantmentInfo? effect)
        => this.IdToEnchantmentMap.TryGetValue(id, out effect);
    
    public EnchantmentInfo GetByName(string name) => this.NameToEnchantmentMap[name];

    public bool TryGetByName(string name, [NotNullWhen(true)] out EnchantmentInfo? effect)
        => this.NameToEnchantmentMap.TryGetValue(name, out effect);
    

    public EnchantmentInfo this[int id] => GetById(id);

    public EnchantmentInfo this[string name] => GetByName(name);
}

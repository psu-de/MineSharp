using MineSharp.Core.Common.Enchantments;
using System.Diagnostics.CodeAnalysis;

namespace MineSharp.Data.Enchantments;

/// <summary>
/// Provides static data for enchantments
/// </summary>
public class EnchantmentProvider : DataProvider<EnchantmentType, EnchantmentInfo>
{
    private Dictionary<int, EnchantmentInfo> IdToEnchantmentMap { get; }
    private Dictionary<string, EnchantmentInfo> NameToEnchantmentMap { get; }
    
    internal EnchantmentProvider(DataVersion<EnchantmentType, EnchantmentInfo> version) : base(version)
    {
        this.IdToEnchantmentMap = version.Palette.ToDictionary(x => x.Value.Id, x => x.Value);
        this.NameToEnchantmentMap = version.Palette.ToDictionary(x => x.Value.Name, x => x.Value);
    }
    
    /// <summary>
    /// Get an EnchantmentInfo by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public EnchantmentInfo GetById(int id) => this.IdToEnchantmentMap[id];
    
    /// <summary>
    /// Try to get an EnchantmentInfo by id. Returns false if not found
    /// </summary>
    /// <param name="id"></param>
    /// <param name="effect"></param>
    /// <returns></returns>
    public bool TryGetById(int id, [NotNullWhen(true)] out EnchantmentInfo? effect)
        => this.IdToEnchantmentMap.TryGetValue(id, out effect);
    
    /// <summary>
    /// Get an EnchantmentInfo by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public EnchantmentInfo GetByName(string name) => this.NameToEnchantmentMap[name];

    /// <summary>
    /// Try to get an EnchantmentInfo by name. Returns false if not found
    /// </summary>
    /// <param name="name"></param>
    /// <param name="effect"></param>
    /// <returns></returns>
    public bool TryGetByName(string name, [NotNullWhen(true)] out EnchantmentInfo? effect)
        => this.NameToEnchantmentMap.TryGetValue(name, out effect);
    
    /// <summary>
    /// Get an EnchantmentInfo by id
    /// </summary>
    /// <param name="id"></param>
    public EnchantmentInfo this[int id] => GetById(id);

    /// <summary>
    /// Get an EnchantmentInfo by name
    /// </summary>
    /// <param name="name"></param>
    public EnchantmentInfo this[string name] => GetByName(name);
}

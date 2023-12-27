using MineSharp.Core.Common.Effects;
using System.Diagnostics.CodeAnalysis;

namespace MineSharp.Data.Effects;

/// <summary>
/// Provides static data about effects
/// Indexes EffectInfo's by id and name
/// </summary>
public class EffectProvider : DataProvider<EffectType, EffectInfo>
{
    
    private Dictionary<int, EffectInfo> IdToEffectMap { get; }
    private Dictionary<string, EffectInfo> NameToEffectMap { get; }
    
    internal EffectProvider(DataVersion<EffectType, EffectInfo> version) : base(version)
    {
        this.IdToEffectMap = version.Palette.ToDictionary(x => x.Value.Id, x => x.Value);
        this.NameToEffectMap = version.Palette.ToDictionary(x => x.Value.Name, x => x.Value);
    }
    
    /// <summary>
    /// Get a EffectInfo by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public EffectInfo GetById(int id) => this.IdToEffectMap[id];
    
    /// <summary>
    /// Try to get an EffectInfo by id. Returns false if not found.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="effect"></param>
    /// <returns></returns>
    public bool TryGetById(int id, [NotNullWhen(true)] out EffectInfo? effect)
        => this.IdToEffectMap.TryGetValue(id, out effect);
    
    /// <summary>
    /// Get an EffectInfo by name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public EffectInfo GetByName(string name) => this.NameToEffectMap[name];

    /// <summary>
    /// Try to get an EffectInfo by name. Returns false if not found.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="effect"></param>
    /// <returns></returns>
    public bool TryGetByName(string name, [NotNullWhen(true)] out EffectInfo? effect)
        => this.NameToEffectMap.TryGetValue(name, out effect);
    

    /// <summary>
    /// Get an EffectInfo by id
    /// </summary>
    /// <param name="id"></param>
    public EffectInfo this[int id] => GetById(id);

    /// <summary>
    /// Get an EffectInfo by name
    /// </summary>
    /// <param name="name"></param>
    public EffectInfo this[string name] => GetByName(name);
}

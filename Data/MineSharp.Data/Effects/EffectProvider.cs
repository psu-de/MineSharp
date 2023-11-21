using MineSharp.Core.Common.Effects;
using System.Diagnostics.CodeAnalysis;

namespace MineSharp.Data.Effects;

public class EffectProvider : DataProvider<EffectType, EffectInfo>
{
    
    private Dictionary<int, EffectInfo> IdToEffectMap { get; }
    private Dictionary<string, EffectInfo> NameToEffectMap { get; }
    
    internal EffectProvider(DataVersion<EffectType, EffectInfo> version) : base(version)
    {
        this.IdToEffectMap = version.Palette.ToDictionary(x => x.Value.Id, x => x.Value);
        this.NameToEffectMap = version.Palette.ToDictionary(x => x.Value.Name, x => x.Value);
    }
    
    public EffectInfo GetById(int id) => this.IdToEffectMap[id];
    
    public bool TryGetById(int id, [NotNullWhen(true)] out EffectInfo? effect)
        => this.IdToEffectMap.TryGetValue(id, out effect);
    
    public EffectInfo GetByName(string name) => this.NameToEffectMap[name];

    public bool TryGetByName(string name, [NotNullWhen(true)] out EffectInfo? effect)
        => this.NameToEffectMap.TryGetValue(name, out effect);
    

    public EffectInfo this[int id] => GetById(id);

    public EffectInfo this[string name] => GetByName(name);
}

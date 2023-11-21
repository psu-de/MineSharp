using MineSharp.Core.Common.Biomes;
using System.Diagnostics.CodeAnalysis;

namespace MineSharp.Data.Biomes;

public class BiomeProvider : DataProvider<BiomeType, BiomeInfo>
{
    private Dictionary<int, BiomeInfo> IdToBiomeMap { get; }
    private Dictionary<string, BiomeInfo> NameToBiomeMap { get; }
    
    public int Count => IdToBiomeMap.Count();
    
    internal BiomeProvider(DataVersion<BiomeType, BiomeInfo> version) : base(version)
    {
        this.IdToBiomeMap = version.Palette.ToDictionary(x => x.Value.Id, x => x.Value);
        this.NameToBiomeMap = version.Palette.ToDictionary(x => x.Value.Name, x => x.Value);
    }

    public BiomeInfo GetById(int id) => this.IdToBiomeMap[id];
    
    public bool TryGetById(int id, [NotNullWhen(true)] out BiomeInfo? biome)
        => this.IdToBiomeMap.TryGetValue(id, out biome);
    
    public BiomeInfo GetByName(string name) => this.NameToBiomeMap[name];

    public bool TryGetByName(string name, [NotNullWhen(true)] out BiomeInfo? biome)
        => this.NameToBiomeMap.TryGetValue(name, out biome);
    

    public BiomeInfo this[int id] => GetById(id);

    public BiomeInfo this[string name] => GetByName(name);
}

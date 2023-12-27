using MineSharp.Core.Common.Biomes;
using System.Diagnostics.CodeAnalysis;

namespace MineSharp.Data.Biomes;

/// <summary>
/// Biome Data Provider.
/// Indexes static <see cref="BiomeInfo"/>'s by id, name and BiomeType
/// </summary>
public class BiomeProvider : DataProvider<BiomeType, BiomeInfo>
{
    private Dictionary<int, BiomeInfo> IdToBiomeMap { get; }
    private Dictionary<string, BiomeInfo> NameToBiomeMap { get; }
    
    /// <summary>
    /// Total count of biomes
    /// </summary>
    public int Count => IdToBiomeMap.Count();
    
    internal BiomeProvider(DataVersion<BiomeType, BiomeInfo> version) : base(version)
    {
        this.IdToBiomeMap = version.Palette.ToDictionary(x => x.Value.Id, x => x.Value);
        this.NameToBiomeMap = version.Palette.ToDictionary(x => x.Value.Name, x => x.Value);
    }

    /// <summary>
    /// Get a <see cref="BiomeInfo"/> by numeric id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public BiomeInfo GetById(int id) => this.IdToBiomeMap[id];
    
    /// <summary>
    /// Try to get a <see cref="BiomeInfo"/> by numeric id. Returns false if not found.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="biome"></param>
    /// <returns></returns>
    public bool TryGetById(int id, [NotNullWhen(true)] out BiomeInfo? biome)
        => this.IdToBiomeMap.TryGetValue(id, out biome);
    
    /// <summary>
    /// Get a <see cref="BiomeInfo"/> by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public BiomeInfo GetByName(string name) => this.NameToBiomeMap[name];

    /// <summary>
    /// Try to get a <see cref="BiomeInfo"/> by name. Returns false if not found.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="biome"></param>
    /// <returns></returns>
    public bool TryGetByName(string name, [NotNullWhen(true)] out BiomeInfo? biome)
        => this.NameToBiomeMap.TryGetValue(name, out biome);
    
    /// <summary>
    /// Get Biome info by id
    /// </summary>
    /// <param name="id"></param>
    public BiomeInfo this[int id] => GetById(id);

    /// <summary>
    /// Get Biome Info by name
    /// </summary>
    /// <param name="name"></param>
    public BiomeInfo this[string name] => GetByName(name);
}

namespace MineSharp.Core.Common.Biomes;

/// <summary>
///     Descriptor class for biomes.
/// </summary>
/// <param name="Id">The numerical id of this biome.</param>
/// <param name="Type">The <see cref="BiomeType" /> of this biome</param>
/// <param name="Name">The text id of this biome</param>
/// <param name="DisplayName"></param>
/// <param name="Category"></param>
/// <param name="Temperature"></param>
/// <param name="Precipitation"></param>
/// <param name="Dimension"></param>
/// <param name="Color"></param>
public record BiomeInfo(
    int Id,
    BiomeType Type,
    string Name,
    string DisplayName,
    BiomeCategory Category,
    float Temperature,
    bool Precipitation,
    Dimension Dimension,
    int Color);

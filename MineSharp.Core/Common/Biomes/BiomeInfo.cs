namespace MineSharp.Core.Common.Biomes;

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

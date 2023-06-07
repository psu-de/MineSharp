namespace MineSharp.Core.Common.Biomes;

public record BiomeInfo(
    int Id,
    string Name,
    string Category,
    float Temperature,
    string Precipitation,
    float Depth,
    string Dimension,
    string DisplayName,
    int Color,
    float Rainfall);

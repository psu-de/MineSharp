namespace MineSharp.Core.Common.Biomes;

/// <summary>
///     Represents a minecraft biome
/// </summary>
public class Biome(BiomeInfo info)
{
    /// <summary>
    ///     Descriptor of this biome
    /// </summary>
    public readonly BiomeInfo Info = info;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Biome (Info={Info})";
    }
}

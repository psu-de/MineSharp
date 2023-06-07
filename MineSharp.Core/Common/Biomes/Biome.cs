namespace MineSharp.Core.Common.Biomes;

public class Biome
{
    public readonly BiomeInfo Info;

    public Biome(BiomeInfo info)
    {
        this.Info = info;
    }

    public override string ToString()
    {
        return $"Biome (Info={this.Info})";
    }
}

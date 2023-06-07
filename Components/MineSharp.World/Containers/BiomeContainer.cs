using MineSharp.Data;
using MineSharp.World.Containers.Palettes;

namespace MineSharp.World.Containers;

internal class BiomeContainer : PaletteContainer
{
    private const byte MAX_BITS = 3;
    private const byte MIN_BITS = 1;
    
    public override int Capacity => 4 * 4 * 4;
    public override byte MinBits => MIN_BITS;
    public override byte MaxBits => MAX_BITS;
    public override int TotalNumberOfStates { get; }
    
    public BiomeContainer(MinecraftData data, IPalette palette, IntBitArray bitData) : base(palette, bitData)
    {
        this.TotalNumberOfStates = data.Biomes.Count;
    }

    public static BiomeContainer FromStream(MinecraftData data, Stream stream)
    {
        (var palette, var bitArray) = PaletteContainer.FromStream(stream, MAX_BITS);
        return new BiomeContainer(data, palette, bitArray);
    }
}

using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.World.Containers.Palettes;

namespace MineSharp.World.Containers;

internal class BiomeContainer : PaletteContainer
{
    private const byte MAX_BITS = 3;
    private const byte MIN_BITS = 1;

    public BiomeContainer(MinecraftData data, IPalette palette, IntBitArray bitData) : base(palette, bitData)
    {
        TotalNumberOfStates = data.Biomes.Count;
    }

    public override int Capacity => 4 * 4 * 4;
    public override byte MinBits => MIN_BITS;
    public override byte MaxBits => MAX_BITS;
    public override int TotalNumberOfStates { get; }

    public static BiomeContainer FromStream(MinecraftData data, PacketBuffer buffer)
    {
        (var palette, var bitArray) = FromStream(buffer, MAX_BITS);
        return new(data, palette, bitArray);
    }
}

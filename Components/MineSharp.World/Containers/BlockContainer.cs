using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.World.Containers.Palettes;

namespace MineSharp.World.Containers;

internal class BlockContainer : PaletteContainer
{
    private const byte MAX_BITS = 8;
    private const byte MIN_BITS = 4;

    public BlockContainer(MinecraftData data, IPalette palette, IntBitArray bitData) : base(palette, bitData)
    {
        TotalNumberOfStates = data.Blocks.TotalBlockStateCount;
    }

    public override int Capacity => 16 * 16 * 16;
    public override byte MinBits => MIN_BITS;
    public override byte MaxBits => MAX_BITS;
    public override int TotalNumberOfStates { get; }

    public static BlockContainer FromStream(MinecraftData data, PacketBuffer buffer)
    {
        (var palette, var bitArray) = FromStream(buffer, MAX_BITS);
        return new(data, palette, bitArray);
    }
}

using MineSharp.Core.Serialization;
using MineSharp.World.Containers.Palettes;

namespace MineSharp.World.Containers;

internal abstract class PaletteContainer : IPaletteContainer
{
    protected PaletteContainer(IPalette palette, IntBitArray data)
    {
        Palette = palette;
        Data = data;
    }

    public IPalette Palette { get; set; }
    public IntBitArray Data { get; set; }
    public abstract int Capacity { get; }
    public abstract int TotalNumberOfStates { get; }
    public abstract byte MinBits { get; }
    public abstract byte MaxBits { get; }

    public int GetAt(int index)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        if (index >= Capacity)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        if (Palette is SingleValuePalette)
        {
            return Palette.Get(0);
        }

        var value = Data.Get(index);
        return Palette.Get(value);
    }

    public void SetAt(int index, int state)
    {
        if (index < 0 || index >= Capacity)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        var newPalette = Palette.Set(index, state, this);
        if (newPalette != null)
        {
            Palette = newPalette;
        }
    }

    protected static (IPalette palette, IntBitArray data) FromStream(PacketBuffer buffer, byte maxBitsPerEntry)
    {
        var bitsPerEntry = buffer.ReadByte();
        IPalette palette;

        if (bitsPerEntry == 0)
        {
            palette = SingleValuePalette.FromStream(buffer);
        }
        else if (bitsPerEntry <= maxBitsPerEntry)
        {
            palette = IndirectPalette.FromStream(buffer);
        }
        else
        {
            palette = DirectPalette.FromStream(buffer);
        }

        var data = buffer.ReadLongArray();

        return (palette, new(data, bitsPerEntry));
    }
}

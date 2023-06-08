using MineSharp.Core.Extensions;
using MineSharp.World.Containers.Palettes;

namespace MineSharp.World.Containers;

internal abstract class PaletteContainer : IPaletteContainer
{
    public IPalette Palette { get; set; }
    public IntBitArray Data { get; set; }
    public abstract int Capacity { get; }
    public abstract int TotalNumberOfStates { get; }
    public abstract byte MinBits { get; }
    public abstract byte MaxBits { get; }

    protected PaletteContainer(IPalette palette, IntBitArray data)
    {
        this.Palette = palette;
        this.Data = data;
    }

    public int GetAt(int index)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (index >= this.Capacity)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (this.Palette is SingleValuePalette)
            return this.Palette.Get(0);

        var value = this.Data.Get(index);
        return this.Palette.Get(value);
    }
    
    public void SetAt(int index, int state)
    {
        if (index < 0 || index >= this.Capacity)
            throw new ArgumentOutOfRangeException(nameof(index));

        this.Palette.Set(index, state, this);
    }

    protected static (IPalette palette, IntBitArray data) FromStream(Stream buffer, byte maxBitsPerEntry)
    {
        var bitsPerEntry = buffer.ReadByte();
        IPalette palette;

        if (bitsPerEntry == 0)
        {
            palette = SingleValuePalette.FromStream(buffer);
        } else if (bitsPerEntry <= maxBitsPerEntry)
        {
            palette = IndirectPalette.FromStream(buffer);
        } else
        {
            palette = DirectPalette.FromStream(buffer);
        }

        var data = new long[buffer.ReadVarInt()];
        for (int i = 0; i < data.Length; i++)
            data[i] = buffer.ReadLong();

        return (palette, new IntBitArray(data, (byte)bitsPerEntry));
    }
}

namespace MineSharp.World.New.Palettes;

internal class BitArray : IBitArray
{
    public long[] Data { get; set; }
    
    public  byte BitsPerEntry { get; set; }

    public int Capacity => ValuesPerLong * Data.Length;

    private int  ValuesPerLong => 64 / BitsPerEntry;
    private long ValueMask     => (1L << BitsPerEntry) - 1;

    public BitArray(long[] data, byte bitsPerEntry)
    {
        Data = data;
        BitsPerEntry = bitsPerEntry;
    }


    public void ChangeBitsPerEntry(byte newBitsPerEntry)
    {
        if (newBitsPerEntry == BitsPerEntry)
        {
            return;
        }

        var old = new BitArray(Data, BitsPerEntry);
        var capacity = Capacity;
        BitsPerEntry = newBitsPerEntry;

        Data = new long[(int)Math.Ceiling((float)capacity / ValuesPerLong)];
        for (var i = 0; i < capacity; i++)
        {
            Set(i, old.Get(i));
        }
    }

    public void Set(int idx, int value)
    {
        if (idx >= Capacity || idx < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(idx));
        }

        var longIndex = idx / ValuesPerLong;
        var bitIndex = idx % ValuesPerLong * BitsPerEntry;

        var l = Data[longIndex];

        var shiftedValue = (value & ValueMask) << bitIndex;

        l |= shiftedValue;

        var mask = ~(ValueMask << bitIndex) | shiftedValue;
        l &= mask;

        Data[longIndex] = l;
    }

    public int Get(int idx)
    {
        if (idx >= Capacity || idx < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(idx));
        }

        var longIndex = idx / ValuesPerLong;
        var bitIndex = idx % ValuesPerLong * BitsPerEntry;

        var l = Data[longIndex];
        return (int)((l >> bitIndex) & ValueMask);
    }
}

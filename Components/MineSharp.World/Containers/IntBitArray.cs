namespace MineSharp.World.Containers;

internal class IntBitArray
{
    public long[] Data         { get; private set; }
    public byte   BitsPerEntry { get; private set; }
    public int    Capacity     => this.ValuesPerLong * this.Data.Length;

    private int  ValuesPerLong => 64 / this.BitsPerEntry;
    private long ValueMask     => (1L << this.BitsPerEntry) - 1;

    public IntBitArray(long[] data, byte bitsPerEntry)
    {
        this.Data         = data;
        this.BitsPerEntry = bitsPerEntry;
    }

    public void ChangeBitsPerEntry(byte newBitsPerEntry)
    {
        if (newBitsPerEntry == this.BitsPerEntry)
            return;

        var old      = new IntBitArray(this.Data, this.BitsPerEntry);
        var capacity = this.Capacity;
        this.BitsPerEntry = newBitsPerEntry;

        this.Data = new long[(int)Math.Ceiling((float)capacity / this.ValuesPerLong)];
        for (var i = 0; i < capacity; i++)
        {
            this.Set(i, old.Get(i));
        }
    }

    public void Set(int idx, int value)
    {
        if (idx >= this.Capacity || idx < 0)
            throw new ArgumentOutOfRangeException(nameof(idx));

        var longIndex = idx                      / this.ValuesPerLong;
        var bitIndex  = idx % this.ValuesPerLong * this.BitsPerEntry;

        var l = this.Data[longIndex];

        var shiftedValue = (value & this.ValueMask) << bitIndex;

        l |= shiftedValue;

        var mask = ~(this.ValueMask << bitIndex) | shiftedValue;
        l &= mask;

        this.Data[longIndex] = l;
    }

    public int Get(int idx)
    {
        if (idx >= this.Capacity || idx < 0)
            throw new ArgumentOutOfRangeException(nameof(idx));

        var longIndex = idx                      / this.ValuesPerLong;
        var bitIndex  = idx % this.ValuesPerLong * this.BitsPerEntry;

        var l = this.Data[longIndex];
        return (int)(l >> bitIndex & this.ValueMask);
    }
}

namespace MineSharp.World.Containers;

internal interface IBitArray
{
    public long[] Data         { get; protected set; }
    public byte   BitsPerEntry { get; protected set; }
    public int    Capacity     { get; }

    public void ChangeBitsPerEntry(byte bitsPerEntry);
    public void Set(int index, int value);
    public void Get(int index);
}

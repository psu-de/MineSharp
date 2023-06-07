using MineSharp.World.Containers.Palettes;

namespace MineSharp.World.Containers;

internal interface IPaletteContainer
{
    public IPalette Palette { get; protected set; }
    public int Capacity { get; }
    public int TotalNumberOfStates { get; }
    public byte MinBits { get; }
    public byte MaxBits { get; }
    
    internal IntBitArray Data { get; set; }

    public int GetAt(int index);
    public void SetAt(int index, int value);
}

namespace MineSharp.World.Containers.Palettes;

internal interface IPalette
{
    public int       Get(int           index);
    public IPalette? Set(int           index,    int state, IPaletteContainer container);
    public bool      ContainsState(int minState, int maxState);
}

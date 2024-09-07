using MineSharp.Core.Serialization;

namespace MineSharp.World.Containers.Palettes;

internal class DirectPalette : IPalette
{
    public int Get(int index)
    {
        return index;
    }

    public bool ContainsState(int minState, int maxState)
    {
        return true;
    }

    public IPalette? Set(int index, int state, IPaletteContainer container)
    {
        container.Data.Set(index, state);
        return null;
    }

    public static IPalette FromStream(PacketBuffer stream)
    {
        return new DirectPalette();
    }
}

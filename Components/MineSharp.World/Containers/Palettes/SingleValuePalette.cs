using MineSharp.Core.Common;

namespace MineSharp.World.Containers.Palettes;

internal class SingleValuePalette : IPalette
{
    public SingleValuePalette(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public bool ContainsState(int minState, int maxState)
    {
        return Value >= minState && Value <= maxState;
    }

    public int Get(int index)
    {
        return Value;
    }

    public IPalette? Set(int index, int state, IPaletteContainer container)
    {
        if (Value == state)
        {
            return null;
        }

        var map = new[] { Value, state };

        var count = (int)Math.Ceiling(container.Capacity / (64.0F / container.MinBits));
        var lData = new long[count];
        container.Data = new(lData, container.MaxBits);
        container.Data.Set(index, 1);

        return new IndirectPalette(map);
    }

    public static IPalette FromStream(PacketBuffer buffer)
    {
        return new SingleValuePalette(buffer.ReadVarInt());
    }
}

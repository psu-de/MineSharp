using MineSharp.Core.Serialization;

namespace MineSharp.World.Containers.Palettes;

internal class IndirectPalette : IPalette
{
    private readonly IList<int> map;

    public IndirectPalette(IList<int> map)
    {
        this.map = map;
    }

    public bool ContainsState(int minState, int maxState)
    {
        for (var i = 0; i < map!.Count; i++)
        {
            if (minState <= map[i] && map[i] <= maxState)
            {
                return true;
            }
        }

        return false;
    }

    public int Get(int index)
    {
        return map[index];
    }

    public IPalette? Set(int index, int state, IPaletteContainer container)
    {
        if (ContainsState(state, state))
        {
            var stateIndex = GetStateIndex(state);
            container.Data.Set(index, stateIndex);
            return null;
        }

        // add the new state to the palette
        var newMapSize = map!.Count + 1;
        var newBitsPerEntry = (byte)Math.Ceiling(Math.Log2(newMapSize));

        if (newBitsPerEntry > container.MaxBits)
        {
            var bits = (int)Math.Ceiling(Math.Log2(container.TotalNumberOfStates));
            var data = new long[bits];
            var newData = new IntBitArray(data, newBitsPerEntry);

            for (var i = 0; i < container.Capacity; i++)
            {
                newData.Set(i, container.GetAt(i));
            }

            newData.Set(index, state);

            container.Data = newData;
            return new DirectPalette();
        }

        container.Data.ChangeBitsPerEntry(newBitsPerEntry);
        container.Data.Set(index, map.Count);

        var newMap = new List<int>(map) { state };
        return new IndirectPalette(newMap);
    }

    public static IPalette FromStream(PacketBuffer stream)
    {
        var map = new int[stream.ReadVarInt()];
        for (var i = 0; i < map.Length; i++)
        {
            map[i] = stream.ReadVarInt();
        }

        return new IndirectPalette(map);
    }

    public int GetStateIndex(int state)
    {
        return map.IndexOf(state);
    }
}

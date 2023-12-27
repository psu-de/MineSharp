using MineSharp.Core.Common;

namespace MineSharp.World.Containers.Palettes;

internal class IndirectPalette : IPalette
{
    private readonly IList<int> _map;

    public IndirectPalette(IList<int> map)
    {
        this._map = map;
    }

    public bool ContainsState(int minState, int maxState)
    {
        for (var i = 0; i < this._map!.Count; i++)
        {
            if (minState <= this._map[i] && this._map[i] <= maxState) 
                return true;
        }

        return false;
    }
    
    public int Get(int index) => this._map[index];

    public IPalette? Set(int index, int state, IPaletteContainer container)
    {
        if (this.ContainsState(state, state))
        {
            var stateIndex = this.GetStateIndex(state);
            container.Data.Set(index, stateIndex);
            return null;
        }

        // add the new state to the palette
        var newMapSize = this._map!.Count + 1;
        var newBitsPerEntry = (byte)Math.Ceiling(Math.Log2(newMapSize));
        
        if (newBitsPerEntry > container.MaxBits)
        {
            var bits = (int)Math.Ceiling(Math.Log2(container.TotalNumberOfStates));
            long[] data = new long[bits];
            var newData = new IntBitArray(data, newBitsPerEntry);

            for (var i = 0; i < container.Capacity; i++)
                newData.Set(i, container.GetAt(i));
            
            newData.Set(index, state);

            container.Data = newData;
            return new DirectPalette();
        }
        container.Data.ChangeBitsPerEntry(newBitsPerEntry);
        container.Data.Set(index, this._map.Count);
        
        var newMap = new List<int>(this._map) { state };
        return new IndirectPalette(newMap);
    }

    public static IPalette FromStream(PacketBuffer stream)
    {
        var map = new int[stream.ReadVarInt()];
        for (int i = 0; i < map.Length; i++)
        {
            map[i] = stream.ReadVarInt();
        }

        return new IndirectPalette(map);
    }

    public int GetStateIndex(int state) => this._map.IndexOf(state);
}

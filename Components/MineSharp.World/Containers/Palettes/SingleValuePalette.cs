using MineSharp.Core.Common;

namespace MineSharp.World.Containers.Palettes;

internal class SingleValuePalette : IPalette
{
    public int Value { get; }

    public SingleValuePalette(int value)
    {
        this.Value = value;
    }

    public bool ContainsState(int minState, int maxState) => this.Value >= minState && this.Value <= maxState;
    
    public int Get(int index) => this.Value;
    
    public IPalette? Set(int index, int state, IPaletteContainer container)
    {
        if (this.Value == state)
        {
            return null;
        }
        
        var map = new[] { this.Value, state };

        var count = (int)Math.Ceiling(container.Capacity / (64.0F / container.MinBits));
        var lData = new long[count];
        container.Data = new IntBitArray(lData, container.MaxBits);
        container.Data.Set(index, 1);
        
        return new IndirectPalette(map);
    }
    
    public static IPalette FromStream(PacketBuffer buffer)
    {
        return new SingleValuePalette(buffer.ReadVarInt());
    }
}
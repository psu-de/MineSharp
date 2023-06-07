using fNbt;

namespace MineSharp.Core.Common.Blocks;

public class BlockEntity
{
    public byte X { get; }
    public short Y { get; }
    public byte Z { get; }
    
    public int Type { get; set; }
    public NbtCompound Data { get; set; }

    public BlockEntity(byte x, short y, byte z, int type, NbtCompound data)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
        this.Type = type;
        this.Data = data;
    }
}

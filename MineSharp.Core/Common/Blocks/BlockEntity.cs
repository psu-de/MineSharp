using fNbt;

namespace MineSharp.Core.Common.Blocks;

/// <summary>
/// A Block entity
/// </summary>
/// <param name="x"></param>
/// <param name="y"></param>
/// <param name="z"></param>
/// <param name="type"></param>
/// <param name="data"></param>
public class BlockEntity(byte x, short y, byte z, int type, NbtCompound data)
{
    /// <summary>
    /// X coordinate
    /// </summary>
    public byte X { get; } = x;

    /// <summary>
    /// Y coordinate
    /// </summary>
    public short Y { get; } = y;

    /// <summary>
    /// Z coordinate
    /// </summary>
    public byte Z { get; } = z;

    /// <summary>
    /// Type
    /// </summary>
    public int Type { get; set; } = type;

    /// <summary>
    /// NBT data
    /// </summary>
    public NbtCompound Data { get; set; } = data;
}

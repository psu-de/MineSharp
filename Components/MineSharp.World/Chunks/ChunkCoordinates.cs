namespace MineSharp.World.Chunks;

/// <summary>
///     Represents a Chunk coordinate
/// </summary>
/// <param name="x"></param>
/// <param name="z"></param>
public readonly struct ChunkCoordinates(int x, int z)
{
    /// <summary>
    ///     The X coordinate
    /// </summary>
    public int X { get; } = x;

    /// <summary>
    ///     The Z coordinate
    /// </summary>
    public int Z { get; } = z;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"({X} / {Z})";
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return (X << 16) | (Z & 0xFFFF);
    }
}

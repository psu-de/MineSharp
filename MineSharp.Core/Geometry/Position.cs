namespace MineSharp.Core.Geometry;

/// <summary>
///     Represents a 3D-Position
/// </summary>
public readonly struct Position : IEquatable<Position>
{
    /// <summary>
    ///     A Position with all coordinates set to zero
    /// </summary>
    public static Position Zero => new(0, 0, 0);

    /// <summary>
    ///     The X coordinate
    /// </summary>
    public readonly int X;

    /// <summary>
    ///     The Y coordinate
    /// </summary>
    public readonly int Y;

    /// <summary>
    ///     The Z coordinate
    /// </summary>
    public readonly int Z;

    /// <summary>
    ///     Create a new Position from a packed ulong <paramref name="value" />
    /// </summary>
    /// <param name="value"></param>
    public Position(ulong value)
    {
        X = (int)(value >> 38);
        Y = (int)(value & 0xFFF);
        Z = (int)((value >> 12) & 0x3FFFFFF);

        if (X >= Math.Pow(2, 25)) { X -= (int)Math.Pow(2, 26); }

        if (Y >= Math.Pow(2, 11)) { Y -= (int)Math.Pow(2, 12); }

        if (Z >= Math.Pow(2, 25)) { Z -= (int)Math.Pow(2, 26); }
    }

    /// <summary>
    ///     Create a new Position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public Position(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>
    ///     Create a new Position from double values.
    ///     The values are floored.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public Position(double x, double y, double z)
    {
        X = (int)Math.Floor(x);
        Y = (int)Math.Floor(y);
        Z = (int)Math.Floor(z);
    }

    /// <summary>
    ///     Check if the two positions represent the same point
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator ==(Position a, Position b)
    {
        return a.Equals(b);
    }

    /// <summary>
    ///     Checks if the two positions do not represent the same point
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator !=(Position a, Position b)
    {
        return !(a == b);
    }

    /// <inheritdoc />
    public bool Equals(Position other)
    {
        return ToULong() == other.ToULong();
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is Position pos)
        {
            return Equals(pos);
        }

        return false;
    }

    /// <summary>
    ///     Pack this Position into a single ulong
    /// </summary>
    /// <returns></returns>
    public ulong ToULong()
    {
        return (((ulong)X & 0x3FFFFFF) << 38) | (((ulong)Z & 0x3FFFFFF) << 12) | ((ulong)Y & 0xFFF);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"({X} / {Y} / {Z})";
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return (X << 22) | ((Z << 12) & 0x3FF) | (Y & 0xFFF);
    }

    /// <summary>
    ///     Convert a Position to a Vector
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static explicit operator Vector3(Position x)
    {
        return new(x.X, x.Y, x.Z);
    }
}

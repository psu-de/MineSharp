using System.Diagnostics.Contracts;

namespace MineSharp.Core.Geometry;

/// <summary>
///     A 3D Vector with double precision floating point coordinates.
/// </summary>
/// <param name="x"></param>
/// <param name="y"></param>
/// <param name="z"></param>
public class Vector3(double x, double y, double z)
{
    /// <summary>
    ///     (1, 1, 1)
    /// </summary>
    public static Vector3 One => new(1, 1, 1);

    /// <summary>
    ///     Zero Vector
    /// </summary>
    public static Vector3 Zero => new(0, 0, 0);

    /// <summary>
    ///     Unit vector pointing up
    /// </summary>
    public static Vector3 Up => new(0, 1, 0);

    /// <summary>
    ///     Unit vector pointing down
    /// </summary>
    public static Vector3 Down => new(0, -1, 0);

    /// <summary>
    ///     Unit vector pointing north
    /// </summary>
    public static Vector3 North => new(0, 0, -1);

    /// <summary>
    ///     Unit vector pointing south
    /// </summary>
    public static Vector3 South => new(0, 0, 1);

    /// <summary>
    ///     Unit vector pointing west
    /// </summary>
    public static Vector3 West => new(-1, 0, 0);

    /// <summary>
    ///     Unit vector pointing east
    /// </summary>
    public static Vector3 East => new(1, 0, 0);

    /// <summary>
    ///     The X coordinate
    /// </summary>
    public double X { get; protected set; } = x;

    /// <summary>
    ///     The Y coordinate
    /// </summary>
    public double Y { get; protected set; } = y;

    /// <summary>
    ///     The Z coordinate
    /// </summary>
    public double Z { get; protected set; } = z;

    /// <summary>
    ///     Returns a new Vector with the <paramref name="other" /> added
    /// </summary>
    [Pure]
    public MutableVector3 Plus(Vector3 other)
    {
        return Clone().Add(other);
    }

    /// <summary>
    ///     Returns a new Vector with the <paramref name="x" /> <paramref name="y" /> <paramref name="z" /> added
    /// </summary>
    public MutableVector3 Plus(double x, double y, double z)
    {
        return Clone().Add(x, y, z);
    }

    /// <summary>
    ///     Returns a new Vector with <paramref name="other" /> subtracted
    /// </summary>
    [Pure]
    public MutableVector3 Minus(Vector3 other)
    {
        return Clone().Subtract(other);
    }

    /// <summary>
    ///     Returns a new Vector with <paramref name="x" /> <paramref name="y" /> <paramref name="z" /> subtracted
    /// </summary>
    [Pure]
    public MutableVector3 Minus(double x, double y, double z)
    {
        return Clone().Subtract(x, y, z);
    }

    /// <summary>
    ///     Returns a new Vector3 multiplied by <paramref name="other" />
    /// </summary>
    /// <param name="other"></param>
    [Pure]
    public MutableVector3 Multiplied(Vector3 other)
    {
        return Clone().Multiply(other);
    }

    /// <summary>
    ///     Returns a new Vector3 divided by <paramref name="other" />
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    [Pure]
    public MutableVector3 Divided(Vector3 other)
    {
        return Clone().Divide(other);
    }

    /// <summary>
    ///     Returns a new Vector which was scaled by <paramref name="scalar" />
    /// </summary>
    /// <param name="scalar"></param>
    /// <returns></returns>
    [Pure]
    public MutableVector3 Scaled(double scalar)
    {
        return Clone().Scale(scalar);
    }

    /// <summary>
    ///     Return a new Vector3 with all values floored
    /// </summary>
    [Pure]
    public MutableVector3 Floored()
    {
        return Clone().Floor();
    }

    /// <summary>
    ///     Returns a new normalized instance of this vector
    /// </summary>
    /// <returns></returns>
    [Pure]
    public MutableVector3 Normalized()
    {
        return Clone().Normalize();
    }

    /// <summary>
    ///     Calculates the length of this vector.
    /// </summary>
    [Pure]
    public double Length()
    {
        return Math.Sqrt(LengthSquared());
    }


    /// <summary>
    ///     Calculates the squared length of this vector instance
    /// </summary>
    [Pure]
    public double LengthSquared()
    {
        return (X * X) + (Y * Y) + (Z * Z);
    }

    /// <summary>
    ///     Calculates the squared horizontal (x, z) length of this vector instance
    /// </summary>
    [Pure]
    public double HorizontalLengthSquared()
    {
        return (X * X) + (Z * Z);
    }

    /// <summary>
    ///     Calculates the horizontal squared length of this vector
    /// </summary>
    [Pure]
    public double HorizontalDistanceToSquared(Vector3 other)
    {
        var dX = X - other.X;
        var dZ = Z - other.Z;
        return (dX * dX) + (dZ * dZ);
    }

    /// <summary>
    ///     Calculates the distance to the <paramref name="other" /> vector.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    [Pure]
    public double DistanceTo(Vector3 other)
    {
        return Minus(other).Length();
    }

    /// <summary>
    ///     Returns the squared distance to the <paramref name="other" /> vector
    /// </summary>
    /// <param name="other"></param>
    /// <returns>The distance squared.</returns>
    [Pure]
    public double DistanceToSquared(Vector3 other)
    {
        return Minus(other).LengthSquared();
    }

    /// <summary>
    ///     Returns this vector cloned
    /// </summary>
    /// <returns></returns>
    [Pure]
    public MutableVector3 Clone()
    {
        return new(X, Y, Z);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"({X:0.#####} / {Y:0.#####} / {Z:0.#####})";
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is not Vector3 vec)
        {
            return false;
        }

        return this == vec;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return X.GetHashCode()
            ^ (Y.GetHashCode() << 2)
            ^ (Z.GetHashCode() >> 2);
    }

    /// <summary>
    ///     Returns a new Position from this vector
    /// </summary>
    public static explicit operator Position(Vector3 x)
    {
        return new(x.X, x.Y, x.Z);
    }

    /// <summary>
    ///     Returns a new Vector from a Position
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static implicit operator Vector3(Position x)
    {
        return new(x.X, x.Y, x.Z);
    }

    /// <summary>
    ///     Checks if the two vectors are equal
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator ==(Vector3 a, Vector3 b)
    {
        return Math.Abs(a.X - b.X) < 1e-7
            && Math.Abs(a.Y - b.Y) < 1e-7
            && Math.Abs(a.Z - b.Z) < 1e-7;
    }

    /// <summary>
    ///     Checks if the two vectors are different.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator !=(Vector3 a, Vector3 b)
    {
        return Math.Abs(a.X - b.X) > 1e-7
            || Math.Abs(a.Y - b.Y) > 1e-7
            || Math.Abs(a.Z - b.Z) > 1e-7;
    }

    /// <summary>
    ///     Component-wise vector addition
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Vector3 operator +(Vector3 a, Vector3 b)
    {
        return a.Plus(b);
    }

    /// <summary>
    ///     Component-wise vector subtraction
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Vector3 operator -(Vector3 a, Vector3 b)
    {
        return a.Minus(b);
    }

    /// <summary>
    ///     Component-wise vector multiplication
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Vector3 operator *(Vector3 a, Vector3 b)
    {
        return a.Multiplied(b);
    }

    /// <summary>
    ///     Component-wise vector division
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Vector3 operator /(Vector3 a, Vector3 b)
    {
        return a.Divided(b);
    }
}

/// <summary>
///     A mutable Vector3
/// </summary>
public class MutableVector3 : Vector3
{
    /// <summary>
    ///     Create a new instance of MutableVector3
    /// </summary>
    public MutableVector3(double x, double y, double z) : base(x, y, z)
    { }

    /// <summary>
    ///     Set x y z coordinates
    /// </summary>
    public MutableVector3 Set(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;

        return this;
    }

    /// <summary>
    ///     Set x z coordinates
    /// </summary>
    public MutableVector3 Set(double x, double z)
    {
        X = x;
        Z = z;

        return this;
    }

    /// <summary>
    ///     Set x coordinate
    /// </summary>
    public MutableVector3 SetX(double x)
    {
        return Set(x, Y, Z);
    }

    /// <summary>
    ///     Set y coordinate
    /// </summary>
    public MutableVector3 SetY(double y)
    {
        return Set(X, y, Z);
    }

    /// <summary>
    ///     Set z coordinate
    /// </summary>
    public MutableVector3 SetZ(double z)
    {
        return Set(X, Y, z);
    }

    /// <summary>
    ///     Component-wise vector addition
    /// </summary>
    public MutableVector3 Add(double x, double y, double z)
    {
        X += x;
        Y += y;
        Z += z;

        return this;
    }

    /// <inheritdoc cref="Add(double, double, double)" />
    public MutableVector3 Add(Vector3 other)
    {
        return Add(other.X, other.Y, other.Z);
    }

    /// <summary>
    ///     Component-wise vector subtraction.
    /// </summary>
    public MutableVector3 Subtract(double x, double y, double z)
    {
        X -= x;
        Y -= y;
        Z -= z;

        return this;
    }

    /// <inheritdoc cref="Subtract(double, double, double)" />
    public MutableVector3 Subtract(Vector3 other)
    {
        return Subtract(other.X, other.Y, other.Z);
    }

    /// <summary>
    ///     Component-wise vector multiplication.
    /// </summary>
    public MutableVector3 Multiply(double x, double y, double z)
    {
        X *= x;
        Y *= y;
        Z *= z;

        return this;
    }

    /// <inheritdoc cref="Multiply(double, double, double)" />
    public MutableVector3 Multiply(Vector3 other)
    {
        return Multiply(other.X, other.Y, other.Z);
    }

    /// <summary>
    ///     Component-wise vector division.
    /// </summary>
    public MutableVector3 Divide(double x, double y, double z)
    {
        X /= x;
        Y /= y;
        Z /= z;

        return this;
    }

    /// <inheritdoc cref="Divide(double, double, double)" />
    public MutableVector3 Divide(Vector3 other)
    {
        return Divide(other.X, other.Y, other.Z);
    }

    /// <summary>
    ///     Scale this vector by a scalar
    /// </summary>
    public MutableVector3 Scale(double scalar)
    {
        return Multiply(scalar, scalar, scalar);
    }

    /// <summary>
    ///     Floor all values of this vector
    /// </summary>
    public MutableVector3 Floor()
    {
        X = Math.Floor(X);
        Y = Math.Floor(Y);
        Z = Math.Floor(Z);

        return this;
    }

    /// <summary>
    ///     Normalizes this vector instance
    /// </summary>
    public MutableVector3 Normalize()
    {
        var length = Length();

        var scale = length == 0
            ? 0
            : 1 / length;

        return Scale(scale);
    }
}

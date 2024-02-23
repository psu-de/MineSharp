using System.Diagnostics.Contracts;

namespace MineSharp.Core.Common;

/// <summary>
/// A 3D Vector with double precision floating point coordinates.
/// </summary>
/// <param name="x"></param>
/// <param name="y"></param>
/// <param name="z"></param>
public class Vector3(double x, double y, double z)
{
    /// <summary>
    /// (1, 1, 1)
    /// </summary>
    public static Vector3 One => new Vector3(1, 1, 1);

    /// <summary>
    /// Zero Vector
    /// </summary>
    public static Vector3 Zero => new Vector3(0, 0, 0);

    /// <summary>
    /// Unit vector pointing up
    /// </summary>
    public static Vector3 Up => new Vector3(0, 1, 0);

    /// <summary>
    /// Unit vector pointing down
    /// </summary>
    public static Vector3 Down => new Vector3(0, -1, 0);

    /// <summary>
    /// Unit vector pointing north
    /// </summary>
    public static Vector3 North => new Vector3(0, 0, -1);

    /// <summary>
    /// Unit vector pointing south
    /// </summary>
    public static Vector3 South => new Vector3(0, 0, 1);

    /// <summary>
    /// Unit vector pointing west
    /// </summary>
    public static Vector3 West => new Vector3(-1, 0, 0);

    /// <summary>
    /// Unit vector pointing east
    /// </summary>
    public static Vector3 East => new Vector3(1, 0, 0);

    /// <summary>
    /// The X coordinate
    /// </summary>
    public double X { get; set; } = x;

    /// <summary>
    /// The Y coordinate
    /// </summary>
    public double Y { get; set; } = y;

    /// <summary>
    /// The Z coordinate
    /// </summary>
    public double Z { get; set; } = z;

    /// <summary>
    /// Component-wise vector addition.
    /// </summary>
    /// <param name="other"></param>
    /// <returns>this</returns>
    public Vector3 Add(Vector3 other)
    {
        this.X += other.X;
        this.Y += other.Y;
        this.Z += other.Z;

        return this;
    }

    /// <summary>
    /// Component-wise vector addition
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns>this</returns>
    public Vector3 Add(double x, double y, double z)
    {
        this.X += x;
        this.Y += y;
        this.Z += z;

        return this;
    }

    /// <summary>
    /// Returns a new Vector with the <paramref name="other"/> added
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    [Pure]
    public Vector3 Plus(Vector3 other)
    {
        return new Vector3(
            this.X + other.X,
            this.Y + other.Y,
            this.Z + other.Z);
    }

    /// <summary>
    /// Component-wise vector subtraction.
    /// </summary>
    /// <param name="other"></param>
    public Vector3 Subtract(Vector3 other)
    {
        this.X -= other.X;
        this.Y -= other.Y;
        this.Z -= other.Z;

        return this;
    }

    /// <summary>
    /// Component-wise vector subtraction.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public Vector3 Subtract(double x, double y, double z)
    {
        this.X -= x;
        this.Y -= y;
        this.Z -= z;

        return this;
    }

    /// <summary>
    /// Returns a new Vector with <paramref name="other"/> subtracted
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    [Pure]
    public Vector3 Minus(Vector3 other)
    {
        return new Vector3(
            this.X - other.X,
            this.Y - other.Y,
            this.Z - other.Z);
    }

    /// <summary>
    /// Component-wise vector multiplication.
    /// </summary>
    /// <param name="other"></param>
    public Vector3 Multiply(Vector3 other)
    {
        this.X *= other.X;
        this.Y *= other.Y;
        this.Z *= other.Z;

        return this;
    }

    /// <summary>
    /// Returns a new Vector3 multiplied by <paramref name="other"/>
    /// </summary>
    /// <param name="other"></param>
    [Pure]
    public Vector3 Multiplied(Vector3 other)
        => this.Clone().Multiply(other);

    /// <summary>
    /// Component-wise vector division.
    /// </summary>
    /// <param name="other"></param>
    public Vector3 Divide(Vector3 other)
    {
        this.X /= other.X;
        this.Y /= other.Y;
        this.Z /= other.Z;

        return this;
    }

    /// <summary>
    /// Returns a new Vector3 divided by <paramref name="other"/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    [Pure]
    public Vector3 Divided(Vector3 other)
        => this.Clone().Divide(other);

    /// <summary>
    /// Scale this vector by a scalar
    /// </summary>
    /// <param name="scalar"></param>
    public void Scale(double scalar)
    {
        this.X *= scalar;
        this.Y *= scalar;
        this.Z *= scalar;
    }

    /// <summary>
    /// Returns a new Vector which was scaled by <paramref name="scalar"/>
    /// </summary>
    /// <param name="scalar"></param>
    /// <returns></returns>
    [Pure]
    public Vector3 Scaled(double scalar)
    {
        var vec = this.Clone();
        vec.Scale(scalar);
        return vec;
    }

    /// <summary>
    /// Floor all values of this vector
    /// </summary>
    /// <returns>this</returns>
    public Vector3 Floor()
    {
        this.X = Math.Floor(this.X);
        this.Y = Math.Floor(this.Y);
        this.Z = Math.Floor(this.Z);

        return this;
    }

    /// <summary>
    /// Return a new Vector3 with all values floored
    /// </summary>
    /// <returns></returns>
    [Pure]
    public Vector3 Floored()
    {
        return this.Clone().Floor();
    }

    /// <summary>
    /// Returns the length of this vector.
    /// </summary>
    /// <returns></returns>
    [Pure]
    public double Length()
    {
        return Math.Sqrt(this.LengthSquared());
    }


    /// <summary>
    /// Returns the squared length of this vector instance
    /// </summary>
    /// <returns></returns>
    [Pure]
    public double LengthSquared()
    {
        return this.X * this.X + this.Y * this.Y + this.Z * this.Z;
    }

    /// <summary>
    /// Returns the horizontal squared length of this vector
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    [Pure]
    public double HorizontalDistanceToSquared(Vector3 other)
    {
        var dX = this.X - other.X;
        var dZ = this.Z - other.Z;
        return dX * dX + dZ + dZ;
    }

    /// <summary>
    /// Returns the distance to the <paramref name="other"/> vector.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    [Pure]
    public double DistanceTo(Vector3 other)
    {
        return this.Minus(other).Length();
    }

    /// <summary>
    /// Returns the squared distance to the <paramref name="other"/> vector
    /// </summary>
    /// <param name="other"></param>
    /// <returns>The distance squared.</returns>
    [Pure]
    public double DistanceToSquared(Vector3 other)
    {
        var diff = this.Minus(other);
        return diff.X * diff.X +
               diff.Y * diff.Y +
               diff.Z * diff.Z;
    }

    /// <summary>
    /// Normalizes this vector instance
    /// </summary>
    public void Normalize()
    {
        var length = this.Length();

        var scale = length == 0
            ? 0
            : 1 / length;
        this.X *= scale;
        this.Y *= scale;
        this.Z *= scale;
    }

    /// <summary>
    /// Returns a new normalized instance of this vector
    /// </summary>
    /// <returns></returns>
    [Pure]
    public Vector3 Normalized()
    {
        var clone = this.Clone();
        clone.Normalize();
        return clone;
    }

    /// <summary>
    /// Returns this vector cloned
    /// </summary>
    /// <returns></returns>
    [Pure]
    public Vector3 Clone()
        => new Vector3(this.X, this.Y, this.Z);

    /// <inheritdoc />
    public override string ToString()
        => $"({this.X:0.#####} / {this.Y:0.#####} / {this.Z:0.#####})";

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is not Vector3 vec)
            return false;

        return this == vec;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return this.X.GetHashCode()
             ^ this.Y.GetHashCode() << 2
             ^ this.Z.GetHashCode() >> 2;
    }

    /// <summary>
    /// Returns a new Position from this vector
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static explicit operator Position(Vector3 x) => new Position(x.X, x.Y, x.Z);

    /// <summary>
    /// Returns a new Vector from a Position
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static implicit operator Vector3(Position x) => new Vector3(x.X, x.Y, x.Z);

    /// <summary>
    /// Checks if the two vectors are equal
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
    /// Checks if the two vectors are different.
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
    /// Component-wise vector addition
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Vector3 operator +(Vector3 a, Vector3 b)
        => a.Plus(b);

    /// <summary>
    /// Component-wise vector subtraction
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Vector3 operator -(Vector3 a, Vector3 b)
        => a.Minus(b);

    /// <summary>
    /// Component-wise vector multiplication
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Vector3 operator *(Vector3 a, Vector3 b)
        => a.Multiplied(b);

    /// <summary>
    /// Component-wise vector division
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Vector3 operator /(Vector3 a, Vector3 b)
        => a.Divided(b);
}

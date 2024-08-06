using System.Diagnostics.Contracts;

namespace MineSharp.Core.Geometry;

/// <summary>
///     3D Axis-Aligned Bounding Box used for all collisions
/// </summary>
public class Aabb
{
    /// <summary>
    ///     Create a new instance of AABB
    /// </summary>
    protected Aabb(Vector3 a, Vector3 b)
    {
        var min = a.Clone();
        var max = b.Clone();

        min.Set(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
        max.Set(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

        Min = min;
        Max = max;
    }

    /// <summary>
    ///     Creates a new AABB
    /// </summary>
    public Aabb(double minX, double minY, double minZ, double maxX, double maxY, double maxZ)
    {
        if (minX > maxX)
        {
            (maxX, minX) = (minX, maxX);
        }

        if (minY > maxY)
        {
            (maxY, minY) = (minY, maxY);
        }

        if (minZ > maxZ)
        {
            (maxZ, minZ) = (minZ, maxZ);
        }

        Min = new MutableVector3(minX, minY, minZ);
        Max = new MutableVector3(maxX, maxY, maxZ);
    }

    /// <summary>
    ///     Width of this bounding box (MaxX - MinX)
    /// </summary>
    public double Width => Math.Abs(Max.X - Min.X);

    /// <summary>
    ///     Height of this bounding box (MaxY - MinY)
    /// </summary>
    public double Height => Math.Abs(Max.Y - Min.Y);

    /// <summary>
    ///     Depth of this bounding box (MaxZ - MinZ)
    /// </summary>
    public double Depth => Math.Abs(Max.Z - Min.Z);

    /// <summary>
    ///     Min X, Y, Z
    /// </summary>
    public virtual Vector3 Min { get; }

    /// <summary>
    ///     Max X, Y, Z
    /// </summary>
    public virtual Vector3 Max { get; }

    /// <summary>
    ///     Returns a clone of this instance.
    /// </summary>
    public MutableAabb Clone()
    {
        return new(Min.X, Min.Y, Min.Z, Max.X, Max.Y, Max.Z);
    }

    /// <summary>
    ///     Whether this bounding box and the other bounding box intersect.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Intersects(Aabb other)
    {
        return Max.X > other.Min.X && Min.X < other.Max.X
            && Max.Y > other.Min.Y && Min.Y < other.Max.Y
            && Max.Z > other.Min.Z && Min.Z < other.Max.Z;
    }

    /// <summary>
    ///     Whether the point <paramref name="point" /> is contained in this bounding box
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    [Pure]
    public bool Contains(Vector3 point)
    {
        return Min.X <= point.X && Max.X > point.X
            && Min.Y <= point.Y && Max.Y > point.Y
            && Min.Z <= point.Z && Max.Z > point.Z;
    }

    /// <summary>
    ///     Checks if the given line starting at <paramref name="origin" /> and moving along <paramref name="direction" />
    ///     intersects this AABB.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public double? IntersectsLine(Vector3 origin, Vector3 direction)
    {
        direction = direction.Normalized();

        var tMinX = (Min.X - origin.X) / direction.X;
        var tMaxX = (Max.X - origin.X) / direction.X;
        var tMinY = (Min.Y - origin.Y) / direction.Y;
        var tMaxY = (Max.Y - origin.Y) / direction.Y;
        var tMinZ = (Min.Z - origin.Z) / direction.Z;
        var tMaxZ = (Max.Z - origin.Z) / direction.Z;

        var tMin = Math.Max(Math.Max(Math.Min(tMinX, tMaxX), Math.Min(tMinY, tMaxY)), Math.Min(tMinZ, tMaxZ));
        var tMax = Math.Min(Math.Min(Math.Max(tMinX, tMaxX), Math.Max(tMinY, tMaxY)), Math.Max(tMinZ, tMaxZ));

        if (tMax < 0 || tMin > tMax)
        {
            return null;
        }
        return tMin;
    }

    /// <summary>
    ///     Returns the min value of the given axis
    /// </summary>
    public double GetMinValue(Axis axis)
    {
        return axis.Choose(Min);
    }

    /// <summary>
    ///     Returns the max value of the given axis
    /// </summary>
    public double GetMaxValue(Axis axis)
    {
        return axis.Choose(Max);
    }

    /// <summary>
    ///     Whether <paramref name="other" /> intersects this bounding box in the given axis
    /// </summary>
    public bool IntersectsAxis(Aabb other, Axis axis)
    {
        var minA = GetMinValue(axis);
        var maxA = GetMaxValue(axis);

        var minB = other.GetMinValue(axis);
        var maxB = other.GetMaxValue(axis);

        return (minA > minB && minA < maxB)
            || (maxA > minB && maxA < maxB)
            || (minB > minA && minB < maxA)
            || (maxB > minA && maxB < maxA)
            || (minA == minB && maxA == maxB);
    }

    /// <summary>
    ///     Calculates the offset to the <paramref name="other" /> aabb on the given axis
    /// </summary>
    public double CalculateMaxOffset(Aabb other, Axis axis, double displacement)
    {
        if (!IntersectsAxis(other, axis.Next) || !IntersectsAxis(other, axis.Previous))
        {
            return displacement;
        }

        var minA = GetMinValue(axis);
        var maxA = GetMaxValue(axis);
        var minB = other.GetMinValue(axis);
        var maxB = other.GetMaxValue(axis);

        if (displacement > 0 && maxB <= minA && maxB + displacement > minA)
        {
            return Math.Clamp(minA - maxB, 0.0, displacement);
        }

        if (displacement < 0 && maxA <= minB && minB + displacement < maxA)
        {
            return Math.Clamp(maxA - minB, displacement, 0.0);
        }

        return displacement;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"AABB ({Min} -> {Max})";
    }
}

/// <summary>
///     A mutable AABB
/// </summary>
public class MutableAabb : Aabb
{
    /// <inheritdoc />
    public MutableAabb(double minX, double minY, double minZ, double maxX, double maxY, double maxZ) : base(
        minX, minY, minZ, maxX, maxY, maxZ)
    {
        Min = (MutableVector3)base.Min;
        Max = (MutableVector3)base.Max;
    }

    /// <inheritdoc />
    public override MutableVector3 Min { get; }

    /// <inheritdoc />
    public override MutableVector3 Max { get; }

    /// <summary>
    ///     Deflate this bounding box by <paramref name="x" />, <paramref name="y" />, <paramref name="z" />
    ///     Mutates this instance.
    /// </summary>
    public MutableAabb Deflate(double x, double y, double z)
    {
        Min.Add(x, y, z);
        Max.Add(-x, -y, -z);

        return this;
    }

    /// <summary>
    ///     Expand this bounding box by x, y, z
    /// </summary>
    public MutableAabb Expand(double x, double y, double z)
    {
        if (x > 0)
        {
            Max.Add(x, 0, 0);
        }
        else
        {
            Min.Add(x, 0, 0);
        }

        if (y > 0)
        {
            Max.Add(0, y, 0);
        }
        else
        {
            Min.Add(0, y, 0);
        }

        if (z > 0)
        {
            Max.Add(0, 0, z);
        }
        else
        {
            Min.Add(0, 0, z);
        }

        return this;
    }

    /// <summary>
    ///     Offset this bounding box by <paramref name="x" />, <paramref name="y" />, <paramref name="z" />.
    ///     Mutates this instance.
    /// </summary>
    public MutableAabb Offset(double x, double y, double z)
    {
        Min.Add(x, y, z);
        Max.Add(x, y, z);

        return this;
    }
}

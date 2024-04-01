using System.Diagnostics.Contracts;

namespace MineSharp.Core.Geometry;

/// <summary>
/// 3D Axis-Aligned Bounding Box used for all collisions
/// </summary>
public class AABB
{

    /// <summary>
    /// Width of this bounding box (MaxX - MinX)
    /// </summary>
    public double Width => Math.Abs(this.Max.X - this.Min.X);

    /// <summary>
    /// Height of this bounding box (MaxY - MinY)
    /// </summary>
    public double Height => Math.Abs(this.Max.Y - this.Min.Y);

    /// <summary>
    /// Depth of this bounding box (MaxZ - MinZ)
    /// </summary>
    public double Depth => Math.Abs(this.Max.Z - this.Min.Z);

    /// <summary>
    /// Min X, Y, Z
    /// </summary>
    public virtual Vector3 Min { get; }

    /// <summary>
    /// Max X, Y, Z
    /// </summary>
    public virtual Vector3 Max { get; }

    /// <summary>
    /// Create a new instance of AABB
    /// </summary>
    protected AABB(Vector3 a, Vector3 b)
    {
        var min = a.Clone();
        var max = b.Clone();
        
        min.Set(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
        max.Set(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
        
        this.Min = min;
        this.Max = max;
    }
    
    /// <summary>
    /// Creates a new AABB
    /// </summary>
    public AABB(double minX, double minY, double minZ, double maxX, double maxY, double maxZ)
    {
        if (minX > maxX)
            (maxX, minX) = (minX, maxX);

        if (minY > maxY)
            (maxY, minY) = (minY, maxY);

        if (minZ > maxZ)
            (maxZ, minZ) = (minZ, maxZ);
        
        this.Min  = new MutableVector3(minX, minY, minZ);
        this.Max  = new MutableVector3(maxX, maxY, maxZ);
    }

    /// <summary>
    /// Returns a clone of this instance.
    /// </summary>
    public MutableAABB Clone()
        => new MutableAABB(this.Min.X, this.Min.Y, this.Min.Z, this.Max.X, this.Max.Y, this.Max.Z);

    /// <summary>
    /// Whether this bounding box and the other bounding box intersect.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Intersects(AABB other)
    {
        return this.Max.X > other.Min.X && this.Min.X < other.Max.X
            && this.Max.Y > other.Min.Y && this.Min.Y < other.Max.Y
            && this.Max.Z > other.Min.Z && this.Min.Z < other.Max.Z;
    }

    /// <summary>
    /// Whether the point <paramref name="point"/> is contained in this bounding box
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    [Pure]
    public bool Contains(Vector3 point)
    {
        return this.Min.X <= point.X && this.Max.X > point.X
            && this.Min.Y <= point.Y && this.Max.Y > point.Y
            && this.Min.Z <= point.Z && this.Max.Z > point.Z;
    }

    /// <summary>
    /// Checks if the given line starting at <paramref name="origin"/> and moving along <paramref name="direction"/>
    /// intersects this AABB.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public bool IntersectsLine(Vector3 origin, Vector3 direction)
    {
        direction = direction.Normalized();

        var tMinX = (this.Min.X - origin.X) / direction.X;
        var tMaxX = (this.Max.X - origin.X) / direction.X;
        var tMinY = (this.Min.Y - origin.Y) / direction.Y;
        var tMaxY = (this.Max.Y - origin.Y) / direction.Y;
        var tMinZ = (this.Min.Z - origin.Z) / direction.Z;
        var tMaxZ = (this.Max.Z - origin.Z) / direction.Z;

        var tMin = Math.Max(Math.Max(Math.Min(tMinX, tMaxX), Math.Min(tMinY, tMaxY)), Math.Min(tMinZ, tMaxZ));
        var tMax = Math.Min(Math.Min(Math.Max(tMinX, tMaxX), Math.Max(tMinY, tMaxY)), Math.Max(tMinZ, tMaxZ));

        return !(tMax < 0 || tMin > tMax);
    }

    /// <summary>
    /// Returns the min value of the given axis
    /// </summary>
    public double GetMinValue(Axis axis)
        => axis.Choose(this.Min);

    /// <summary>
    /// Returns the max value of the given axis
    /// </summary>
    public double GetMaxValue(Axis axis)
        => axis.Choose(this.Max);

    /// <summary>
    /// Whether <paramref name="other"/> intersects this bounding box in the given axis
    /// </summary>
    public bool IntersectsAxis(AABB other, Axis axis)
    {
        var minA = this.GetMinValue(axis);
        var maxA = this.GetMaxValue(axis);

        var minB = other.GetMinValue(axis);
        var maxB = other.GetMaxValue(axis);

        return minA > minB  && minA < maxB
            || maxA > minB  && maxA < maxB
            || minB > minA  && minB < maxA
            || maxB > minA  && maxB < maxA
            || minA == minB && maxA == maxB;
    }

    /// <summary>
    /// Calculates the offset to the <paramref name="other"/> aabb on the given axis
    /// </summary>
    public double CalculateMaxOffset(AABB other, Axis axis, double displacement)
    {
        if (!IntersectsAxis(other, axis.Next) || !IntersectsAxis(other, axis.Previous))
        {
            return displacement;
        }
        
        var minA = this.GetMinValue(axis);
        var maxA = this.GetMaxValue(axis);
        var minB = other.GetMinValue(axis);
        var maxB = other.GetMaxValue(axis);

        if (displacement > 0 && maxB <= minA && maxB + displacement > minA)
            return Math.Clamp(minA - maxB, 0.0, displacement);

        if (displacement < 0 && maxA <= minB && minB + displacement < maxA)
            return Math.Clamp(maxA - minB, displacement, 0.0);

        return displacement;
    }

    /// <inheritdoc />
    public override string ToString() =>
        $"AABB ({Min} -> {Max})";
}

/// <summary>
/// A mutable AABB
/// </summary>
public class MutableAABB : AABB
{
    /// <inheritdoc />
    public override MutableVector3 Min { get; }
    
    /// <inheritdoc />
    public override MutableVector3 Max { get; }


    /// <inheritdoc />
    public MutableAABB(double minX, double minY, double minZ, double maxX, double maxY, double maxZ) : base(minX, minY, minZ, maxX, maxY, maxZ)
    {
        this.Min = (MutableVector3)base.Min;
        this.Max = (MutableVector3)base.Max;
    }
    
    /// <summary>
    /// Deflate this bounding box by <paramref name="x"/>, <paramref name="y"/>, <paramref name="z"/>
    /// Mutates this instance.
    /// </summary>
    public MutableAABB Deflate(double x, double y, double z)
    {
        this.Min.Add(x, y, z);
        this.Max.Add(-x, -y, -z);
        
        return this;
    }

    /// <summary>
    /// Expand this bounding box by x, y, z
    /// </summary>
    public MutableAABB Expand(double x, double y, double z)
    {
        if (x > 0)
            this.Max.Add(x, 0, 0);
        else this.Min.Add(x, 0, 0);

        if (y > 0)
            this.Max.Add(0, y, 0);
        else this.Min.Add(0, y, 0);

        if (z > 0)
            this.Max.Add(0, 0, z);
        else this.Min.Add(0, 0, z);

        return this;
    }

    /// <summary>
    /// Offset this bounding box by <paramref name="x"/>, <paramref name="y"/>, <paramref name="z"/>.
    /// Mutates this instance.
    /// </summary>
    public MutableAABB Offset(double x, double y, double z)
    {
        this.Min.Add(x, y, z);
        this.Max.Add(x, y, z);
        
        return this;
    }
}

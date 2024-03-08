using System.Diagnostics.Contracts;

namespace MineSharp.Core.Common;

/// <summary>
/// 3D Axis-Aligned Bounding Box used for all collisions
/// </summary>
public class AABB
{
    /// <summary>
    /// Lower X coordinate
    /// </summary>
    public double MinX => this.Min.X;

    /// <summary>
    /// Lower Y coordinate
    /// </summary>
    public double MinY => this.Min.Y;

    /// <summary>
    /// Lower Z coordinate
    /// </summary>
    public double MinZ => this.Min.Z;

    /// <summary>
    /// Upper X coordinate
    /// </summary>
    public double MaxX => this.Max.X;

    /// <summary>
    /// Upper Y coordinate
    /// </summary>
    public double MaxY => this.Max.Y;

    /// <summary>
    /// Upper Z coordinate
    /// </summary>
    public double MaxZ => this.Max.Z;

    /// <summary>
    /// Width of this bounding box (MaxX - MinX)
    /// </summary>
    public double Width => this.MaxX - this.MinX;

    /// <summary>
    /// Height of this bounding box (MaxY - MinY)
    /// </summary>
    public double Height => this.MaxY - this.MinY;

    /// <summary>
    /// Depth of this bounding box (MaxZ - MinZ)
    /// </summary>
    public double Depth => this.MaxZ - this.MinZ;

    /// <summary>
    /// Min X, Y, Z
    /// </summary>
    public Vector3 Min { get; }

    /// <summary>
    /// Max X, Y, Z
    /// </summary>
    public Vector3 Max { get; }
    
    /// <summary>
    /// Creates a new AABB
    /// </summary>
    /// <param name="minX"></param>
    /// <param name="minY"></param>
    /// <param name="minZ"></param>
    /// <param name="maxX"></param>
    /// <param name="maxY"></param>
    /// <param name="maxZ"></param>
    public AABB(double minX, double minY, double minZ, double maxX, double maxY, double maxZ)
    {
        if (minX > maxX)
            (maxX, minX) = (minX, maxX);

        if (minY > maxY)
            (maxY, minY) = (minY, maxY);

        if (minZ > maxZ)
            (maxZ, minZ) = (minZ, maxZ);
        
        this.Min  = new Vector3(minX, minY, minZ);
        this.Max  = new Vector3(maxX, maxY, maxZ);
    }


    /// <summary>
    /// Deflate this bounding box by <paramref name="x"/>, <paramref name="y"/>, <paramref name="z"/>
    /// Mutates this instance.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public AABB Deflate(double x, double y, double z)
    {
        this.Min.Add(x, y, z);
        this.Max.Add(-x, -y, -z);
        
        return this;
    }

    public AABB Extend(double x, double y, double z)
    {
        if (x > 0)
            this.Max.X  += x;
        else this.Min.X += x;

        if (y > 0)
            this.Max.Y  += y;
        else this.Min.Y += y;

        if (z > 0)
            this.Max.Z  += z;
        else this.Min.Z += z;

        return this;
    }

    /// <summary>
    /// Offset this bounding box by <paramref name="x"/>, <paramref name="y"/>, <paramref name="z"/>.
    /// Mutates this instance.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public AABB Offset(double x, double y, double z)
    {
        this.Min.Add(x, y, z);
        this.Max.Add(x, y, z);
        
        return this;
    }

    /// <summary>
    /// Returns a clone of this instance.
    /// </summary>
    /// <returns></returns>
    public AABB Clone()
        => new AABB(this.MinX, this.MinY, this.MinZ, this.MaxX, this.MaxY, this.MaxZ);

    /// <summary>
    /// Whether this bounding box and the other bounding box intersect.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Intersects(AABB other)
    {
        return this.MaxX > other.MinX && this.MinX < other.MaxX
            && this.MaxY > other.MinY && this.MinY < other.MaxY
            && this.MaxZ > other.MinZ && this.MinZ < other.MaxZ;
    }

    /// <summary>
    /// Whether the point <paramref name="point"/> is contained in this bounding box
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    [Pure]
    public bool Contains(Vector3 point)
    {
        return this.MinX <= point.X && this.MaxX > point.X
            && this.MinY <= point.Y && this.MaxY > point.Y
            && this.MinZ <= point.Z && this.MaxZ > point.Z;
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

        var tMinX = (this.MinX - origin.X) / direction.X;
        var tMaxX = (this.MaxX - origin.X) / direction.X;
        var tMinY = (this.MinY - origin.Y) / direction.Y;
        var tMaxY = (this.MaxY - origin.Y) / direction.Y;
        var tMinZ = (this.MinZ - origin.Z) / direction.Z;
        var tMaxZ = (this.MaxZ - origin.Z) / direction.Z;

        var tMin = Math.Max(Math.Max(Math.Min(tMinX, tMaxX), Math.Min(tMinY, tMaxY)), Math.Min(tMinZ, tMaxZ));
        var tMax = Math.Min(Math.Min(Math.Max(tMinX, tMaxX), Math.Max(tMinY, tMaxY)), Math.Max(tMinZ, tMaxZ));

        return !(tMax < 0 || tMin > tMax);
    }

    /// <inheritdoc />
    public override string ToString() =>
        $"AABB ({Min} -> {Max})";
}

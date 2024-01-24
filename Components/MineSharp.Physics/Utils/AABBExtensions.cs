using MineSharp.Core.Common;

namespace MineSharp.Physics.Utils;

internal static class AABBExtensions
{
    public static double CalculateMaxOffset(this AABB aabb, AABB other, Axis axis, double displacement)
    {
        if (!IntersectsAxis(aabb, other, axis.Next) || !IntersectsAxis(aabb, other, axis.Previous))
            return displacement;

        var minA = axis.GetBBMin(aabb);
        var maxA = axis.GetBBMax(aabb);
        var minB = axis.GetBBMin(other);
        var maxB = axis.GetBBMax(other);

        if (displacement > 0 && maxB <= minA && maxB + displacement > minA)
            return Math.Clamp(minA - maxB, 0.0, displacement);

        if (displacement < 0 && maxA <= minB && minB + displacement < maxA)
            return Math.Clamp(maxA - minB, displacement, 0.0);

        return displacement;
    }

    public static AABB ExpandedBoundingBox(this AABB aabb, double x, double y, double z)
    {
        double minX = aabb.MinX;
        double minY = aabb.MinY;
        double minZ = aabb.MinZ;
        double maxX = aabb.MaxX;
        double maxY = aabb.MaxY;
        double maxZ = aabb.MaxZ;
        
        if (x < 0.0D) 
            minX += x; 
        else
            maxX += x;

        if (y < 0.0D)
            minY += y;
        else 
            maxY += y;
        
        if (z < 0.0D)
            minZ += z;
        else if (z > 0.0D)
            maxZ += z;

        return new AABB(minX, minY, minZ, maxX, maxY, maxZ);
    }
    
    public static bool IntersectsAxis(this AABB aabb, AABB b, Axis axis)
    {
        var minA = axis.GetBBMin(aabb);
        var maxA = axis.GetBBMax(aabb);

        var minB = axis.GetBBMin(b);
        var maxB = axis.GetBBMax(b);

        return minA > minB && minA < maxB
               || maxA > minB && maxA < maxB
               || minB > minA && minB < maxA
               || maxB > minA && maxB < maxA
               || minA == minB && maxA == maxB;
    }
}
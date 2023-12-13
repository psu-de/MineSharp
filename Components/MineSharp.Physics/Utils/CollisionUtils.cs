using MineSharp.Core.Common;
using System.Diagnostics;

namespace MineSharp.Physics.Utils;

internal static class CollisionUtils
{
    public static readonly Vector3[] XZAxisVectors = new[] {
        Vector3.West, Vector3.East, Vector3.North, Vector3.South
    };

    public static double HorizontalDistanceSquared(Vector3 vec)
    {
        return vec.X * vec.X + vec.Z * vec.Z;
    }
    
    public static double ChooseAxisCoordinate(Vector3 axis, double x, double y, double z)
    {
        if (axis.X != 0.0)
            return x;
        if (axis.Y != 0.0)
            return y;
        if (axis.Z != 0.0)
            return z;

        throw new InvalidOperationException("null vector as axis");
    }

    public static bool IsPositiveAxisVector(Vector3 axis)
    {
        if (axis.X != 0)
            return axis.X > 0;
        if (axis.Y != 0)
            return axis.Y > 0;
        if (axis.Z != 0)
            return axis.Z > 0;

        throw new ArgumentException("null vector as axis");
    }

    public static double CalculateMaxOffset(AABB a, AABB b, Axis axis, double offset)
    {
        if (!IntersectsAxis(a, b, NextAxis(axis)) || !IntersectsAxis(a, b, PrevAxis(axis)))
            return offset;

        var minA = GetBBMin(a, axis);
        var maxA = GetBBMax(a, axis);
        var minB = GetBBMin(b, axis);
        var maxB = GetBBMin(b, axis);

        return offset switch {
            > 0 when maxB <= minA && maxB + offset > minA => Math.Clamp(minA - maxB, 0.0, offset),
            < 0 when maxA <= minB && minB + offset < maxA => Math.Clamp(maxA - minB, offset, 0.0),
            _ => offset
        };
    }

    public static double GetBBMax(AABB bb, Axis axis)
    {
        return axis switch {
            Axis.XAxis => bb.MaxX,
            Axis.YAxis => bb.MaxY,
            Axis.ZAxis => bb.MaxZ,
            _ => throw new UnreachableException()
        };
    }
    
    public static double GetBBMin(AABB bb, Axis axis)
    {
        return axis switch {
            Axis.XAxis => bb.MinX,
            Axis.YAxis => bb.MinY,
            Axis.ZAxis => bb.MinZ,
            _ => throw new UnreachableException()
        };
    }

    public static AABB ExpandBoundingBox(AABB bb, double x, double y, double z)
    {
        double minX = bb.MinX;
        double minY = bb.MinY;
        double minZ = bb.MinZ;
        double maxX = bb.MaxX;
        double maxY = bb.MaxY;
        double maxZ = bb.MaxZ;
        
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
    
    private static bool IntersectsAxis(AABB a, AABB b, Axis axis)
    {
        var minA = GetBBMin(a, axis);
        var maxA = GetBBMax(a, axis);

        var minB = GetBBMin(b, axis);
        var maxB = GetBBMax(b, axis);

        return minA >= minB && minA <= maxB
               || maxA >= minB && maxA <= maxB
               || minB >= minA && minB <= maxA
               || maxB >= minA && maxB <= maxA
               || minA == minA && maxA == maxB;
    }

    private static Axis NextAxis(Axis axis)
    {
        return axis switch {
            Axis.XAxis => Axis.YAxis,
            Axis.YAxis => Axis.ZAxis,
            Axis.ZAxis => Axis.XAxis,
            _ => throw new UnreachableException()
        };
    }

    private static Axis PrevAxis(Axis axis)
    {
        return axis switch {
            Axis.XAxis => Axis.ZAxis,
            Axis.YAxis => Axis.XAxis,
            Axis.ZAxis => Axis.YAxis,
            _ => throw new UnreachableException()
        };
    }
    
    public enum Axis
    {
        XAxis,
        YAxis,
        ZAxis
    }
}

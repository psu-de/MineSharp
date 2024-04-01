using MineSharp.Core.Geometry;

namespace MineSharp.Physics.Utils;

internal static class VectorExtensions
{
    public static double HorizontalLengthSquared(this Vector3 vector)
    {
        return vector.X * vector.X + vector.Z * vector.Z;
    }

    public static bool IsPositiveAxisVector(this Vector3 axis)
    {
        if (axis.X != 0)
            return axis.X > 0;
        if (axis.Y != 0)
            return axis.Y > 0;
        if (axis.Z != 0)
            return axis.Z > 0;

        throw new ArgumentException("null vector as axis");
    }

    public static double ChooseValueForAxis(this Vector3 axis, double x, double y, double z)
    {
        if (axis.X != 0)
            return x;
        if (axis.Y != 0)
            return y;
        if (axis.Z != 0)
            return z;

        throw new ArgumentException("null vector as axis vector");
    }
}

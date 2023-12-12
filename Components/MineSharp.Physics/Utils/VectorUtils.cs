using MineSharp.Core.Common;

namespace MineSharp.Physics.Utils;

public static class VectorUtils
{
    public static readonly Vector3[] XZAxisVectors = new[] {
        Vector3.West, Vector3.East, Vector3.North, Vector3.South
    };
    
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
}

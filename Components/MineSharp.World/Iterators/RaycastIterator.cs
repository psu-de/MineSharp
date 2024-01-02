using MineSharp.Core.Common;

namespace MineSharp.World.Iterators;

/// <summary>
/// Iterate through all block positions that are on a straight line.
/// Note: Does not check whether the ray actually intersects with the
/// bounding box of the block of the yielded block positions
/// </summary>
public class RaycastIterator(Vector3 origin, Vector3 direction, double? length = null) : IWorldIterator
{
    private Vector3 direction = direction.Normalized();
    private Vector3 current = origin.Clone();

    private double maxLength = length ?? double.MaxValue;
    private double length = 0;

    /// <inheritdoc />
    public IEnumerable<Position> Iterate()
    {
        while (length < maxLength)
        {
            yield return Step();
        }
    }

    private Position Step()
    {
        var nextX = GetCoordinateOfNextFace(current.X, direction.X);
        var deltaX = Math.Abs(nextX - current.X);
        var nX = deltaX / Math.Abs(direction.X);
        
        var nextY = GetCoordinateOfNextFace(current.Y, direction.Y);
        var deltaY = Math.Abs(nextY - current.Y);
        var nY = deltaY / Math.Abs(direction.Y);
        
        var nextZ = GetCoordinateOfNextFace(current.Z, direction.Z);
        var deltaZ = Math.Abs(nextZ - current.Z);
        var nZ = deltaZ / Math.Abs(direction.Z);

        var min = Math.Min(nX, Math.Min(nY, nZ));

        var add = direction.Scaled(min + 1e-5);
        var pos = current.Plus(direction.Scaled(min + 1e-5));
        
        this.length += add.Length();
        this.current = pos;
        
        return (Position)pos;
    }

    private double GetCoordinateOfNextFace(double position, double step)
    {
        return step >= 0 
            ? Math.Floor(position + 1) 
            : Math.Ceiling(position - 1);
    }
}
using MineSharp.Core.Geometry;

namespace MineSharp.World.Iterators;

/// <summary>
///     Iterate through all block positions that are on a straight line.
///     Note: Does not check whether the ray actually intersects with the
///     bounding box of the block of the yielded block positions
/// </summary>
public class RaycastIterator(Vector3 origin, Vector3 direction, double? length = null) : IWorldIterator
{
    private readonly MutableVector3 current = origin.Clone();
    private readonly Vector3 direction = direction.Normalized();

    private readonly double maxLength = length ?? double.MaxValue;
    private double length;

    /// <summary>
    ///     The BlockFace the ray hit
    /// </summary>
    public BlockFace CurrentFace { get; private set; }

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
        var nX = CalculateOffset(current.X, direction.X);
        var nY = CalculateOffset(current.Y, direction.Y);
        var nZ = CalculateOffset(current.Z, direction.Z);

        var min = Math.Min(nX, Math.Min(nY, nZ));

        CurrentFace = GetFace(nX, nY, nZ);

        var add = direction.Scaled(min + 1e-7);
        current.Add(add);

        length += add.Length();

        return (Position)current;
    }

    private BlockFace GetFace(double x, double y, double z)
    {
        (var face, var step) = x < y
            ? x < z
                ? (BlockFace.West, direction.X)
                : (BlockFace.North, direction.Z)
            : y < z
                ? (BlockFace.Bottom, direction.Y)
                : (BlockFace.North, direction.Z);

        if (step < 0)
        {
            face += 1;
        }

        return face;
    }

    private double CalculateOffset(double position, double step)
    {
        var next = GetCoordinateOfNextFace(position, step);
        var delta = Math.Abs(next - position);
        return delta / Math.Abs(step);
    }

    private double GetCoordinateOfNextFace(double position, double step)
    {
        return step >= 0
            ? Math.Floor(position + 1)
            : Math.Ceiling(position - 1);
    }
}

using MineSharp.Core.Geometry;

namespace MineSharp.World.Iterators;

/// <summary>
///     The Bounding Box iterator returns all Positions that may
///     collide with the bounding box.
/// </summary>
public class BoundingBoxIterator : IWorldIterator
{
    private readonly XyzIterator iterator;

    /// <summary>
    ///     Create a new instance
    /// </summary>
    /// <param name="aabb"></param>
    public BoundingBoxIterator(Aabb aabb)
    {
        var minX = (int)Math.Floor(aabb.Min.X - 1.0E-7D) - 1;
        var maxX = (int)Math.Floor(aabb.Max.X + 1.0E-7D) + 1;
        var minY = (int)Math.Floor(aabb.Min.Y - 1.0E-7D) - 1;
        var maxY = (int)Math.Floor(aabb.Max.Y + 1.0E-7D) + 1;
        var minZ = (int)Math.Floor(aabb.Min.Z - 1.0E-7D) - 1;
        var maxZ = (int)Math.Floor(aabb.Max.Z + 1.0E-7D) + 1;

        iterator = new(
            new(minX, minY, minZ),
            new(maxX, maxY, maxZ));
    }

    /// <inheritdoc />
    public IEnumerable<Position> Iterate()
    {
        return iterator.Iterate();
    }
}

using MineSharp.Core.Common;
using MineSharp.Core.Geometry;

namespace MineSharp.World.Iterators;

/// <summary>
/// The Bounding Box iterator returns all Positions that may
/// collide with the bounding box.
/// </summary>
public class BoundingBoxIterator : IWorldIterator
{
    private XYZIterator _iterator;

    /// <summary>
    /// Create a new instance
    /// </summary>
    /// <param name="aabb"></param>
    public BoundingBoxIterator(AABB aabb)
    {
        var minX = (int)Math.Floor(aabb.Min.X - 1.0E-7D) - 1;
        var maxX = (int)Math.Floor(aabb.Max.X + 1.0E-7D) + 1;
        var minY = (int)Math.Floor(aabb.Min.Y - 1.0E-7D) - 1;
        var maxY = (int)Math.Floor(aabb.Max.Y + 1.0E-7D) + 1;
        var minZ = (int)Math.Floor(aabb.Min.Z - 1.0E-7D) - 1;
        var maxZ = (int)Math.Floor(aabb.Max.Z + 1.0E-7D) + 1;

        this._iterator = new XYZIterator(
            new Position(minX, minY, minZ),
            new Position(maxX, maxY, maxZ));
    }

    /// <inheritdoc />
    public IEnumerable<Position> Iterate() => this._iterator.Iterate();
}

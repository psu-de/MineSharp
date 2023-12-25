using MineSharp.Core.Common;

namespace MineSharp.World.Iterators;

public class BoundingBoxIterator : IWorldIterator
{
    private SpiralIterator _iterator;
       
    public BoundingBoxIterator(AABB aabb)
    {
        var minX = (int)Math.Floor(aabb.MinX - 1.0E-7D) - 1;
        var maxX = (int)Math.Floor(aabb.MaxX + 1.0E-7D) + 1;
        var minY = (int)Math.Floor(aabb.MinY - 1.0E-7D) - 1;
        var maxY = (int)Math.Floor(aabb.MaxY + 1.0E-7D) + 1;
        var minZ = (int)Math.Floor(aabb.MinZ - 1.0E-7D) - 1;
        var maxZ = (int)Math.Floor(aabb.MaxZ + 1.0E-7D) + 1;
        
        this._iterator = new SpiralIterator(
            new Position(minX, minY, minZ),
            new Position(maxX, maxY, maxZ));
    }

    public IEnumerable<Position> Iterate() => this._iterator.Iterate();
}

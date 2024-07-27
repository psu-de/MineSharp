using MineSharp.Core.Geometry;

namespace MineSharp.World.Iterators;

/// <summary>
///     The Spiral iterator iterates through all positions by incrementing X, then Y and then Z.
/// </summary>
public class XyzIterator : IWorldIterator
{
    private readonly int depth;
    private readonly int end;
    private readonly int height;
    private readonly Position origin;
    private readonly int width;

    private int index;
    private int x;
    private int y;
    private int z;

    /// <summary>
    ///     Create a new instance.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="to"></param>
    public XyzIterator(Position origin, Position to)
    {
        this.origin = origin;
        width = to.X - origin.X;
        height = to.Y - origin.Y;
        depth = to.Z - origin.Z;
        end = width * height * depth;
    }

    /// <inheritdoc />
    public IEnumerable<Position> Iterate()
    {
        while (Advance())
        {
            yield return new(
                origin.X + x,
                origin.Y + y,
                origin.Z + z);
        }
    }

    private bool Advance()
    {
        if (index == end)
        {
            return false;
        }

        var i = index / width;
        x = index % width;
        y = i % height;
        z = i / height;
        index++;
        return true;
    }
}

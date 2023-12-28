using MineSharp.Core.Common;

namespace MineSharp.World.Iterators;

/// <summary>
/// The Spiral iterator iterates through all positions by incrementing X, then Y and then Z.
/// </summary>
public class XYZIterator : IWorldIterator
{
    private readonly Position origin;
    private readonly int width;
    private readonly int height;
    private readonly int depth;
    private readonly int end;
    
    private int index;
    private int x;
    private int y;
    private int z;

    /// <summary>
    /// Create a new instance.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="to"></param>
    public XYZIterator(Position origin, Position to)
    {
        this.origin = origin;
        this.width = to.X - origin.X;
        this.height = to.Y - origin.Y;
        this.depth = to.Z - origin.Z;
        this.end = this.width * this.height * this.depth;
        
    }
    
    /// <inheritdoc />
    public IEnumerable<Position> Iterate()
    {
        while (this.Advance())
        {
            yield return new Position(
                this.origin.X + this.x, 
                this.origin.Y + this.y, 
                this.origin.Z + this.z);
        }
    }

    private bool Advance()
    {
        if (this.index == this.end)
            return false;

        var i = this.index / this.width;
        this.x = this.index % this.width;
        this.y = i % this.height;
        this.z = i / this.height;
        this.index++;
        return true;
    }
}

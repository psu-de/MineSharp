using MineSharp.Core.Common;

namespace MineSharp.World.Iterators;

public class SpiralIterator : IWorldIterator
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

    public SpiralIterator(Position origin, Position to)
    {
        this.origin = origin;
        this.width = to.X - origin.X;
        this.height = to.Y - origin.Y;
        this.depth = to.Z - origin.Z;
        this.end = this.width * this.height * this.depth;
        
    }
    
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

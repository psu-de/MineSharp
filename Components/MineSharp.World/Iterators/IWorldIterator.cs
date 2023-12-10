using MineSharp.Core.Common;

namespace MineSharp.World.Iterators;

public interface IWorldIterator
{
    public IEnumerable<Position> Iterate();
}

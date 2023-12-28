using MineSharp.Core.Common;

namespace MineSharp.World.Iterators;

/// <summary>
/// A World iterator.
/// </summary>
public interface IWorldIterator
{
    /// <summary>
    /// Yields an IEnumerable of positions.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Position> Iterate();
}

using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Geometry;
using MineSharp.World.Chunks;

namespace MineSharp.World;

/// <summary>
/// Event delegates for MineSharp.World
/// </summary>
public static class Events
{
    /// <summary>
    /// A chunk delegate
    /// </summary>
    public delegate void ChunkEvent(IWorld sender, IChunk chunk);

    /// <summary>
    /// A block delegate
    /// </summary>
    public delegate void BlockEvent(IWorld sender, Block block);

    /// <summary>
    /// A chunk block update delegate
    /// </summary>
    public delegate void ChunkBlockEvent(IChunk sender, int newState, Position position);
}

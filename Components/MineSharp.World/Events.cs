using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.World.Chunks;

namespace MineSharp.World;

public static class Events
{
    public delegate void ChunkEvent(IWorld sender, IChunk chunk);

    public delegate void BlockEvent(IWorld sender, Block block);

    public delegate void ChunkBlockEvent(IChunk sender, int newState, Position position);
}

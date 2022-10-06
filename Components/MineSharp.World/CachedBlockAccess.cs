using MineSharp.Core.Types;
using System.Collections.Concurrent;

namespace MineSharp.World
{
    /// <summary>
    /// Saves blocks from World.GetBlockAt(pos) into a cache, to return it faster.
    /// Does not include <see cref="World.FindBlockAsync(Data.Blocks.BlockType, CancellationToken?)"/>
    /// </summary>
    public class TemporaryBlockCache : IDisposable
    {
        public ConcurrentDictionary<ulong, Block> Cache { get; set; }

        private World World { get; set; }

        public TemporaryBlockCache(World world)
        {
            if (world.TempCache != null)
            {
                throw new InvalidOperationException($"World is already using a {nameof(TemporaryBlockCache)}");
            }

            Cache = new ConcurrentDictionary<ulong, Block>();
            World = world;
            World.TempCache = this;
        }

        public void Dispose()
        {
            World.TempCache = null;
        }
    }
}

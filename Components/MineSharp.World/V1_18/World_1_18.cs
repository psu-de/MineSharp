using MineSharp.Core.Common;
using MineSharp.Core.Common.Biomes;
using MineSharp.Core.Common.Blocks;
using MineSharp.Data;
using MineSharp.World.Chunks;
using MineSharp.World.Exceptions;
using MineSharp.World.Iterators;
using NLog;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace MineSharp.World.V1_18;

/// <summary>
/// World implementation for >= 1.18
/// </summary>
public class World_1_18 : AbstractWorld
{
    internal const int WORLD_HEIGHT = MAX_Y - MIN_Y;
    internal const int MIN_Y        = -64;
    internal const int MAX_Y        = 320;

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger(typeof(IWorld));

    /// <inheritdoc />
    public override int MaxY => MAX_Y;

    /// <inheritdoc />
    public override int MinY => MIN_Y;

    /// <inheritdoc />
    public World_1_18(MinecraftData data) : base(data)
    { }

    /// <inheritdoc />
    public override bool IsOutOfMap(Position position)
    {
        if (position.Y <= MinY || position.Y >= MaxY)
            return true;

        if (Math.Abs(position.X) >= 29999984)
            return true;

        if (Math.Abs(position.Z) >= 29999984)
            return true;

        return false;
    }

    /// <inheritdoc />
    public override IChunk CreateChunk(ChunkCoordinates coordinates, BlockEntity[] entities)
    {
        return new Chunk_1_18(this.Data, coordinates, entities);
    }
}

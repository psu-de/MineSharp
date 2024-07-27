﻿using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Geometry;
using MineSharp.Data;
using MineSharp.World.Chunks;
using NLog;

namespace MineSharp.World.V1_18;

/// <summary>
///     World implementation for >= 1.18
/// </summary>
public class World118 : AbstractWorld
{
    internal const int WorldHeight = MAX_Y - MIN_Y;
    internal const int MAX_Y = 320;
    internal const int MIN_Y = -64;
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger(typeof(IWorld));


    /// <inheritdoc />
    public World118(MinecraftData data) : base(data)
    { }

    /// <inheritdoc />
    public override int MaxY => MAX_Y;

    /// <inheritdoc />
    public override int MinY => MIN_Y;

    /// <inheritdoc />
    public override bool IsOutOfMap(Position position)
    {
        if (position.Y <= MinY || position.Y >= MaxY)
        {
            return true;
        }

        if (Math.Abs(position.X) >= 29999984)
        {
            return true;
        }

        if (Math.Abs(position.Z) >= 29999984)
        {
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override IChunk CreateChunk(ChunkCoordinates coordinates, BlockEntity[] entities)
    {
        return new Chunk118(Data, coordinates, entities);
    }
}

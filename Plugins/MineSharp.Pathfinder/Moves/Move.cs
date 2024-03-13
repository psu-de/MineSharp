using MineSharp.Core.Common;
using MineSharp.Bot;
using MineSharp.Bot.Plugins;
using MineSharp.Core.Common.Entities;
using MineSharp.Core.Geometry;
using MineSharp.Data;
using MineSharp.Pathfinder.Utils;
using MineSharp.Physics.Input;
using MineSharp.World;

namespace MineSharp.Pathfinder.Moves;

/// <summary>
/// Interface for creating a move
/// </summary>
public abstract class Move
{
    /// <summary>
    /// Relative movement vector for this move
    /// </summary>
    public abstract Vector3 Motion { get; }
    
    /// <summary>
    /// Cost for A*. A higher cost means the move will be performed less likely.
    /// </summary>
    public abstract float Cost { get; }

    /// <summary>
    /// Whether multiple instances of this move can be
    /// joint together.
    /// </summary>
    public abstract bool CanBeLinked { get; }
    
    /// <summary>
    /// Checks if the move is possible
    /// </summary>
    public abstract bool IsMovePossible(Position position, IWorld world);

    internal async Task Perform(MineSharpBot bot, int count, Position startPosition, Movements movements)
    {
        if (!this.CanBeLinked && count > 1)
        {
            throw new ArgumentException("count cannot be greater than 1", nameof(count));
        }

        var player  = bot.GetPlugin<PlayerPlugin>();
        var physics = bot.GetPlugin<PhysicsPlugin>();
        var entity  = player.Entity ?? throw new NullReferenceException("player is not initialized");

        if (entity.Velocity.HorizontalLengthSquared() > 0.15 * 0.15)
            await MovementUtils.SlowDown(entity, physics); 
        
        await physics.Look(0, 0);
        await MovementUtils.MoveInsideBlock(entity, startPosition, physics);

        await PerformMove(bot, count, movements);
    }

    /// <summary>
    /// Perform the move. Before PerformMove() is called, it is made guaranteed that:
    ///  - The bot is looking at (0, 0)
    ///  - The bots hitbox is completely inside the block
    ///  - The bots velocity is lower than 0.15^2
    ///  - The input controls are reset
    /// </summary>
    protected abstract Task PerformMove(MineSharpBot bot, int count, Movements movements);
}

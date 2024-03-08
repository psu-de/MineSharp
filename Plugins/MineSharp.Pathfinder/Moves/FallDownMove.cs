using MineSharp.Bot;
using MineSharp.Bot.Plugins;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Data;
using MineSharp.Pathfinder.Utils;
using MineSharp.World;
using MineSharp.World.Iterators;

namespace MineSharp.Pathfinder.Moves;

/// <summary>
/// Fall down to an adjacent block 
/// </summary>
public class FallDownMove(Vector3 motion) : IMove
{
    /// <inheritdoc />
    public Vector3 Motion { get; } = Vector3.Down.Plus(motion);
    
    /// <inheritdoc />
    public float Cost => 2;
    
    /// <inheritdoc />
    public bool CanBeLinked => false;

    /// <inheritdoc />
    public bool IsMovePossible(Position position, IWorld world)
    {
        var playerBb = CollisionHelper.SetAABBToPlayerBB(position);
        playerBb.Offset(
            0.5 + this.Motion.X / 2, 
            0, 
            0.5 + this.Motion.Z / 2);

        return !CollisionHelper.CollidesWithWord(playerBb, world);
    }

    /// <inheritdoc />
    public async Task PerformMove(MineSharpBot bot, int count, Movements movements)
    {
        var physics = bot.GetPlugin<PhysicsPlugin>();
        var player  = bot.GetPlugin<PlayerPlugin>();
        var entity  = player.Entity ?? throw new NullReferenceException("Player entity not initialized.");
        
        var target = player.Self!.Entity!.Position
            .Floored()
            .Add(this.Motion)
            .Add(0.5, 0.0, 0.5);
        var targetBlock = (Position)target;
        
        MovementUtils.SetHorizontalMovementsFromVector(this.Motion, physics.InputControls);

        var stopNextTick = false;
        while (player.Entity.IsOnGround)
        {
            if (!stopNextTick)
            {
                var vec = MovementUtils.GetXZPositionNextTick(player.Entity);
                if (CollisionHelper.IsXZPositionInBlock(vec, targetBlock))
                {
                    stopNextTick = true;
                }
            }
            
            await physics.WaitForTick();
            
            if (stopNextTick)
                physics.InputControls.Reset();
        }
        
        physics.InputControls.Reset();
        await physics.WaitForOnGround();

        if (!CollisionHelper.IsOnPosition(entity.Position, targetBlock))
        {
            throw new Exception("move went wrong."); // TODO: Better exception
        }

        await MovementUtils.MoveToBlockCenter(player.Self, physics);
    }

    private bool ReachedTargetEdge(double edge, double pos, double vel)
    {
        var positionNextTick = pos + vel;

        return vel > 0
            ? positionNextTick > edge || Math.Abs(edge - positionNextTick) <= 0.05
            : positionNextTick < edge || Math.Abs(edge - positionNextTick) <= 0.05;
    }

    private double GetBoundingBoxEdge(double axisMotion)
        => axisMotion > 0 ? 0.3 : 0.7;
}

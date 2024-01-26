using MineSharp.Bot;
using MineSharp.Bot.Plugins;
using MineSharp.Core.Common;
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
    public bool IsMovePossible(Position position, IWorld world, MinecraftData data)
    {
        var playerBb = CollisionHelper.GetPlayerBoundingBox(position);
        playerBb.Offset(
            0.5 + this.Motion.X / 2, 
            -1, 
            0.5 + this.Motion.Z / 2);

        return new BoundingBoxIterator(playerBb)
            .Iterate()
            .Select(world.GetBlockAt)
            .Select(x => CollisionHelper.GetBoundingBoxes(x, data))
            .SelectMany(x => x)
            .Any(x => x.Intersects(playerBb));    
    }

    public async Task PerformMove(MineSharpBot bot, int count, Movements movements)
    {
        var physics = bot.GetPlugin<PhysicsPlugin>();
        var player = bot.GetPlugin<PlayerPlugin>();

        var target = player.Self!.Entity!.Position
            .Floored()
            .Add(this.Motion)
            .Add(0.5, 0.0, 0.5);
        
        var targetEdge = target.Floored();
        if (this.Motion.X != 0)
            targetEdge.Add(GetBoundingBoxEdge(this.Motion.X), 0.0, 0.0);
        if (this.Motion.Z != 0)
            targetEdge.Add(0.0, 0.0, GetBoundingBoxEdge(this.Motion.Z));
        
        await physics.LookAt(target);
        
        var wasFalling = false;
        var entity = player.Entity!;
        
        physics.InputControls.Reset();
        physics.InputControls.ForwardKeyDown = true;
        Console.WriteLine($"FallDownMove! Motion = {this.Motion} TargetEdge = {targetEdge}");
        while (true)
        {
            await physics.WaitForTick();

            if (!wasFalling && !entity.IsOnGround)
            {
                physics.InputControls.ForwardKeyDown = false;
                physics.InputControls.BackwardKeyDown = false;
                wasFalling = true;
            }
            
            if (!wasFalling && !physics.InputControls.BackwardKeyDown)
            {
                var reachedDx = this.Motion.X == 0 ||
                                ReachedTargetEdge(targetEdge.X, entity.Position.X, entity.Velocity.X);
                var reachedDz = this.Motion.Z == 0 ||
                                ReachedTargetEdge(targetEdge.Z, entity.Position.Z, entity.Velocity.Z);

                if (reachedDx && reachedDz)
                {
                    physics.InputControls.BackwardKeyDown = true;
                }
            }

            if (wasFalling && entity.IsOnGround)
            {
                physics.ForceLookAt(target); // TODO: Only force look at when target angle is close
                physics.InputControls.ForwardKeyDown = true;
            }
            
            var dst = target.DistanceToSquared(player.Self!.Entity!.Position);
            if (dst <= IMove.THRESHOLD_COMPLETED)
                break;
        }
        
        physics.InputControls.Reset();
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

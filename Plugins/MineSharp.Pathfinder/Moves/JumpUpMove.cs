using MineSharp.Bot;
using MineSharp.Bot.Plugins;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Pathfinder.Utils;
using MineSharp.World;
using MineSharp.World.Iterators;

namespace MineSharp.Pathfinder.Moves;

/// <summary>
/// Jump up to an adjacent block
/// </summary>
/// <param name="xzMotion"></param>
public class JumpUpMove(Vector3 xzMotion) : IMove
{
    /// <inheritdoc />
    public Vector3 Motion { get; } = Vector3.Up.Plus(xzMotion);
    
    /// <inheritdoc />
    public float Cost => 20;

    /// <inheritdoc />
    public bool CanBeLinked => false;

    /// <inheritdoc />
    public bool IsMovePossible(Position position, IWorld world, MinecraftData data)
    {
        var playerBb = CollisionHelper.GetPlayerBoundingBox(position);
        playerBb.Offset(
            0.5 + this.Motion.X / 2, 
            1, 
            0.5 + this.Motion.Z / 2);

        return new BoundingBoxIterator(playerBb)
            .Iterate()
            .Select(world.GetBlockAt)
            .Select(x => CollisionHelper.GetBoundingBoxes(x, data))
            .SelectMany(x => x)
            .Any(x => x.Intersects(playerBb));
    }


    /// <inheritdoc />
    public async Task PerformMove(MineSharpBot bot, int count, Movements movements)
    {
        if (count != 1)
            throw new InvalidOperationException();

        var physics = bot.GetPlugin<PhysicsPlugin>();
        var player = bot.GetPlugin<PlayerPlugin>();

        var target = player.Self!.Entity!.Position
            .Floored()
            .Add(this.Motion)
            .Add(0.5, 0.0, 0.5);

        await physics.LookAt(target);

        physics.InputControls.ForwardKeyDown = true;
        physics.InputControls.JumpingKeyDown = true;
        physics.InputControls.SprintingKeyDown = movements.AllowSprinting && Math.Abs(this.Motion.LengthSquared() - 1) < 0.01;

        var prevDst = double.MaxValue;
        
        while (true)
        {
            await physics.WaitForTick();
            if (!player.Entity!.IsOnGround)
            {
                physics.InputControls.JumpingKeyDown = false;
                await physics.WaitForOnGround();
                physics.InputControls.ForwardKeyDown = false;
                await physics.LookAt(target);
                physics.InputControls.ForwardKeyDown = true;
            }
            
            // keep jumping if below target block
            physics.InputControls.JumpingKeyDown = player.Entity.Position.Y - target.Y < 0.0;
            
            var dst = player.Self!.Entity!.Position
                .DistanceToSquared(target);

            if (dst > prevDst)
                await physics.LookAt(target);

            if (dst <= IMove.THRESHOLD_COMPLETED)
            {
                break;
            }
        }
        
        physics.InputControls.Reset();
    }
}

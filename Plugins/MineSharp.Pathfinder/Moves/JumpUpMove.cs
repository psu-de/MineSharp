using MineSharp.Bot;
using MineSharp.Bot.Plugins;
using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
using MineSharp.Data;
using MineSharp.Pathfinder.Utils;
using MineSharp.World;
using MineSharp.World.Iterators;

namespace MineSharp.Pathfinder.Moves;

/// <summary>
/// Jump up to an adjacent block
/// </summary>
/// <param name="xzMotion"></param>
public class JumpUpMove(Vector3 xzMotion) : Move
{
    /// <inheritdoc />
    public override Vector3 Motion { get; } = Vector3.Up.Plus(xzMotion);
    
    /// <inheritdoc />
    public override float Cost => 20;

    /// <inheritdoc />
    public override bool CanBeLinked => false;

    /// <inheritdoc />
    public override bool IsMovePossible(Position position, IWorld world)
    {
        var playerBb = CollisionHelper.GetAabbForPlayer(position);
        playerBb.Offset(
            0.5 + this.Motion.X / 2, 
            1, 
            0.5 + this.Motion.Z / 2);

        return !CollisionHelper.CollidesWithWord(playerBb, world);
    }


    /// <inheritdoc />
    protected override async Task PerformMove(MineSharpBot bot, int count, Movements movements)
    {
        var physics = bot.GetPlugin<PhysicsPlugin>();
        var player  = bot.GetPlugin<PlayerPlugin>();
        var entity  = player.Entity ?? throw new NullReferenceException("player is not initialized");

        var target = player.Self!.Entity!.Position
            .Floored()
            .Add(this.Motion)
            .Add(0.5, 0.0, 0.5);

        var targetBlock = (Position)target;
        
        MovementUtils.SetHorizontalMovementsFromVector(this.Motion, physics.InputControls);
        await physics.WaitForTick(); // wait one tick to assure velocity is updated to this moves motion
        physics.InputControls.JumpingKeyDown = true;
        
        while (true)
        {
            await physics.WaitForTick();

            if (CollisionHelper.IntersectsBbWithBlock(entity.GetBoundingBox(), targetBlock))
            {
                break;
            }
        }
        
        physics.InputControls.Reset();
        await physics.WaitForOnGround();

        await MovementUtils.MoveInsideBlock(entity, targetBlock, physics);
    }
}

using MineSharp.Bot;
using MineSharp.Bot.Plugins;
using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
using MineSharp.Pathfinder.Utils;
using MineSharp.World;

namespace MineSharp.Pathfinder.Moves;

/// <summary>
/// Jump over one block
/// </summary>
public class JumpOneBlock(Vector3 direction) : Move
{
    /// <inheritdoc />
    public override Vector3 Motion { get; } = direction.Scaled(2);

    /// <inheritdoc />
    public override float Cost => 100;

    /// <inheritdoc />
    public override bool CanBeLinked => false;
    
    /// <inheritdoc />
    public override bool IsMovePossible(Position position, IWorld world)
    {
        var playerBb = CollisionHelper.SetAABBToPlayerBB(position);
        playerBb.Offset(
            0.5 + this.Motion.X / 2, 
            0, 
            0.5 + this.Motion.Z / 2);
        
        playerBb.Expand(
            this.Motion.X / 2, 
            1, 
            this.Motion.Z / 2);

        return !CollisionHelper.CollidesWithWord(playerBb, world);
    }
    
    /// <inheritdoc />
    protected override async Task PerformMove(MineSharpBot bot, int count, Movements movements)
    {
        var player  = bot.GetPlugin<PlayerPlugin>();
        var physics = bot.GetPlugin<PhysicsPlugin>();
        var entity  = player.Entity ?? throw new NullReferenceException("entity in null");

        var target = entity.Position
                           .Floored()
                           .Add(0.5, 0, 0.5)
                           .Add(this.Motion);
        var jumpBlock = (Position)entity.Position
                                        .Floored()
                                        .Add(
                                             this.Motion.X / 2, 
                                             0, 
                                             this.Motion.Z / 2);
        var targetBlock = (Position)target;

        await physics.Look(0, 0);
        await MovementUtils.MoveToBlockCenter(entity, physics);
        
        MovementUtils.SetHorizontalMovementsFromVector(this.Motion, physics.InputControls);

        while (true)
        {
            if (CollisionHelper.IsPositionInBlock(entity.Position, jumpBlock))
            {
                physics.InputControls.JumpingKeyDown = true;
                await physics.WaitForTick();
                
                break;
            }

            await physics.WaitForTick();
        }

        while (true)
        {
            if (CollisionHelper.IsPositionInBlock(entity.Position.Plus(entity.Velocity), jumpBlock))
            {
                physics.InputControls.Reset();
                break;
            }

            await physics.WaitForTick();
        }
        
        await physics.WaitForOnGround();

        if (!CollisionHelper.IsPositionInBlock(entity.Position, targetBlock))
        {
            throw new Exception("move went wrong."); // TODO: Better exception
        }

        await MovementUtils.MoveToBlockCenter(entity, physics);
    }
}

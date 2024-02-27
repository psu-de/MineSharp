using MineSharp.Bot;
using MineSharp.Bot.Plugins;
using MineSharp.Core.Common;
using MineSharp.Pathfinder.Utils;
using MineSharp.World;

namespace MineSharp.Pathfinder.Moves;

public class JumpOneBlock(Vector3 direction) : IMove
{
    /// <inheritdoc />
    public Vector3 Motion { get; } = direction.Scaled(2);

    /// <inheritdoc />
    public float Cost => 10;

    /// <inheritdoc />
    public bool CanBeLinked => false;
    
    /// <inheritdoc />
    public bool    IsMovePossible(Position position, IWorld world)
    {
        var playerBb = CollisionHelper.GetPlayerBoundingBox(position);
        playerBb.Offset(
            0.5 + this.Motion.X / 2, 
            0, 
            0.5 + this.Motion.Z / 2);
        
        playerBb.Extend(
            this.Motion.X / 2, 
            1, 
            this.Motion.Z / 2);

        return !CollisionHelper.CollidesWithWord(playerBb, world);
    }
    
    /// <inheritdoc />
    public async Task PerformMove(MineSharpBot bot, int count, Movements movements)
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
        
        MovementUtils.SetHorizontalMovementsFromVector(this.Motion, physics.InputControls);

        while (true)
        {
            var x = entity.Position.X + entity.Velocity.X;
            var z = entity.Position.Z + entity.Velocity.Z;

            if (CollisionHelper.IsOnPositionXZ(x, z, jumpBlock))
            {
                physics.InputControls.JumpingKeyDown = true;
                await physics.WaitForTick();
                
                break;
            }

            await physics.WaitForTick();
        }

        while (true)
        {
            var x = entity.Position.X + entity.Velocity.X;
            var z = entity.Position.Z + entity.Velocity.Z;

            if (CollisionHelper.IsOnPositionXZ(x, z, targetBlock))
            {
                physics.InputControls.Reset();
                break;
            }

            await physics.WaitForTick();
        }
        
        await physics.WaitForOnGround();

        if (!CollisionHelper.IsOnPosition(entity.Position, targetBlock))
        {
            throw new Exception("move went wrong."); // TODO: Better exception
        }

        await MovementUtils.MoveToBlockCenter(player.Self!, physics);
    }
}

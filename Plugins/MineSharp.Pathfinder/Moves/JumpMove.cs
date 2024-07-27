using MineSharp.Bot;
using MineSharp.Bot.Plugins;
using MineSharp.Core.Geometry;
using MineSharp.Data;
using MineSharp.Pathfinder.Exceptions;
using MineSharp.Pathfinder.Utils;
using MineSharp.World;

namespace MineSharp.Pathfinder.Moves;

/// <summary>
/// A move that includes jumping
/// </summary>
public class JumpMove : Move
{
    /// <inheritdoc />
    public override Vector3 Motion { get; }

    /// <inheritdoc />
    public override float Cost { get; }

    /// <inheritdoc />
    public override bool CanBeLinked { get; } = false;

    /// <summary>
    /// Create a new instance of JumpMove
    /// </summary>
    public JumpMove(Vector3 motion)
    {
        if (motion.Y is < 0 or > 1)
        {
            throw new ArgumentException("y motion must be 0 or 1", nameof(motion));
        }
        
        this.Motion = motion;
        this.Cost   = 10.0f * (float)motion.Length();
    }
    
    /// <inheritdoc />
    public override bool IsMovePossible(Position position, IWorld world, MinecraftData data)
    {
        var x = this.Motion.X == 0 ? 0 : this.Motion.X < 0 ? -1 : 1;
        var z = this.Motion.Z == 0 ? 0 : this.Motion.Z < 0 ? -1 : 1; 
        
        var pos = new MutableVector3(0.5, 0, 0.5).Add(position);
        
        var motionBelow = this.Motion
                              .Clone()
                              .Subtract(x, 0, z);

        var bb = CollisionHelper.GetAabbForPlayer(pos)
                                .Expand(motionBelow.X, 0, motionBelow.Z);
        
        if (CollisionHelper.CollidesWithWord(bb, world, data))
            return false;

        bb = CollisionHelper.SetAabbToPlayerBB(pos, bb)
                            .Offset(this.Motion.X, this.Motion.Y, this.Motion.Z);
        
        return !CollisionHelper.CollidesWithWord(bb, world, data);
    }
    
    /// <inheritdoc />
    protected override async Task PerformMove(MineSharpBot bot, int count, Movements movements)
    {
        if (this.Motion.Y > 0 && Math.Abs(this.Motion.X) <= 1 && Math.Abs(this.Motion.Z) <= 1)
        {
            await JumpUpToAdjacentBlock(bot, movements);
            return;
        }
        
        var physics = bot.GetPlugin<PhysicsPlugin>();
        var player  = bot.GetPlugin<PlayerPlugin>();
        var entity  = player.Entity ?? throw new NullReferenceException("player is not initialized");
        
        var target = player.Self!.Entity!.Position
                           .Floored()
                           .Add(this.Motion)
                           .Add(0.5, 0.0, 0.5);

        var targetBlock = (Position)target;
        var jumpBlock   = (Position)entity.Position.Minus(0, -1, 0); // the block below the entity is where we jump from
        
        physics.InputControls.SprintingKeyDown = this.Motion.Length() > 2 && movements.AllowSprinting;
        if (physics.InputControls.SprintingKeyDown)
        {
            // if we want to sprint, we have to rotate
            await MovementUtils.SetRotationForMotion(this.Motion, physics);
            physics.InputControls.ForwardKeyDown = true;
        }
        else
        {
            MovementUtils.SetHorizontalMovementsFromVector(this.Motion, entity.Yaw, physics.InputControls);
        }

        await physics.WaitForTick();
        
        var bb     = entity.GetBoundingBox();
        while (true)
        {
            await physics.WaitForTick();
            CollisionHelper.SetAabbToPlayerBB(MovementUtils.GetPositionNextTick(entity), bb);

            if (bb.Min.Y - target.Y + this.Motion.Y < -1)
            {
                throw new MoveWentWrongException("jump move went wrong");
            }
            
            if (CollisionHelper.IntersectsBbWithBlock(bb, jumpBlock))
            {
                continue;
            }

            // if the entity doesn't collide with the block on the next tick, jump                
            physics.InputControls.JumpingKeyDown = true;
            break;
        }

        while (entity.IsOnGround)
        {
            await physics.WaitForTick();
        }
        physics.InputControls.JumpingKeyDown = false;
        
        while (true)
        {
            await physics.WaitForTick();
            
            CollisionHelper.SetAabbToPlayerBB(MovementUtils.GetPositionNextTick(entity), bb);
            if (CollisionHelper.IntersectsBbWithBlockXz(bb, targetBlock))
            {
                break;
            }
        }
        
        physics.InputControls.Reset();

        if (entity.Velocity.LengthSquared() > 0.1 * 0.1)
        {
            await MovementUtils.SlowDown(entity, physics);
        }
        
        await physics.WaitForOnGround();
    }
    
    private async Task JumpUpToAdjacentBlock(MineSharpBot bot, Movements movements)
    {
        var physics = bot.GetPlugin<PhysicsPlugin>();
        var player  = bot.GetPlugin<PlayerPlugin>();
        var entity  = player.Entity ?? throw new NullReferenceException("player is not initialized");

        var target = player.Self!.Entity!.Position
                           .Floored()
                           .Add(this.Motion)
                           .Add(0.5, 0.0, 0.5);

        var targetBlock = (Position)target;
        
        MovementUtils.SetHorizontalMovementsFromVector(this.Motion, entity.Yaw, physics.InputControls);
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

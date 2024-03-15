using MineSharp.Bot;
using MineSharp.Bot.Plugins;
using MineSharp.Core.Geometry;
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
    public override bool IsMovePossible(Position position, IWorld world)
    {
        var motionBelow = this.Motion.Clone().Subtract(
            this.Motion.X < 0 ? -1 : 1, 
            0,
            this.Motion.Z < 0 ? -1 : 1);

        var bb = CollisionHelper.GetAabbForPlayer(position)
                                .Expand(motionBelow.X, 0, motionBelow.Z);

        if (CollisionHelper.CollidesWithWord(bb, world))
            return false;

        bb = CollisionHelper.SetAABBToPlayerBB(position, ref bb)
                            .Offset(this.Motion.X, this.Motion.Y, this.Motion.Z);
        
        return !CollisionHelper.CollidesWithWord(bb, world);
    }
    
    /// <inheritdoc />
    protected override async Task PerformMove(MineSharpBot bot, int count, Movements movements)
    {
        if (this.Motion.Y > 0 && this.Motion.LengthSquared() <= 3)
        {
            await JumpUpDirectly(bot, movements);
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
        var jumpBlock   = (Position)entity.Position.Subtract(0, -1, 0); // the block below the entity is where we jump from
        
        MovementUtils.SetHorizontalMovementsFromVector(this.Motion, physics.InputControls);
        await physics.WaitForTick();
        physics.InputControls.SprintingKeyDown = movements.AllowSprinting;
        
        var bb     = entity.GetBoundingBox();
        while (true)
        {
            await physics.WaitForTick();
            CollisionHelper.SetAABBToPlayerBB(MovementUtils.GetPositionNextTick(entity), ref bb);

            if (bb.Min.Y - target.Y < -1)
            {
                throw new MoveWentWrongException("jump move went wrong");
            }
            
            if (CollisionHelper.IntersectsBbWithBlock(bb, jumpBlock))
            {
                continue;
            }

            // if the entity doesn't collide with the block on the next tick, jump                
            physics.InputControls.JumpingKeyDown = true;
            Console.WriteLine("Jumping");
            break;
        }

        while (entity.IsOnGround)
        {
            await physics.WaitForTick();
            physics.InputControls.JumpingKeyDown = false;
            Console.WriteLine("Left ground");
        }

        while (true)
        {
            await physics.WaitForTick();
            
            CollisionHelper.SetAABBToPlayerBB(MovementUtils.GetPositionNextTick(entity), ref bb);
            if (!CollisionHelper.IntersectsBbWithBlock(bb, targetBlock))
            {
                continue;
            }
            
            physics.InputControls.Reset();
            await physics.WaitForOnGround();
            break;
        }
    }
    
    private async Task JumpUpDirectly(MineSharpBot bot, Movements movements)
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

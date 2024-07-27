using MineSharp.Bot.Plugins;
using MineSharp.Core.Common.Entities;
using MineSharp.Core.Geometry;
using MineSharp.Pathfinder.Exceptions;
using MineSharp.Physics.Input;

namespace MineSharp.Pathfinder.Utils;

internal static class MovementUtils
{
    public static async Task SetRotationForMotion(Vector3 movement, PhysicsPlugin physics)
    {
        var yaw = -Math.Atan2(movement.X, movement.Z) / Math.PI * 180.0;
        await physics.Look((float)yaw, physics.Engine!.Player.Entity!.Pitch);
    }
    
    public static void SetHorizontalMovementsFromVector(Vector3 movement, double yaw, InputControls controls)
    {
        var vec = movement.Clone();
        RotateVector(vec, -yaw);
        
        controls.Reset();
        
        if (vec.Z > 0)
            controls.ForwardKeyDown = true;

        if (vec.Z < 0)
            controls.BackwardKeyDown = true;

        if (vec.X > 0)
            controls.LeftKeyDown = true;

        if (vec.X < 0)
            controls.RightKeyDown = true;
    }

    public static async Task SlowDown(Entity entity, PhysicsPlugin physics)
    {
        physics.InputControls.Reset();
        for (int i = 0; i < 10; i++)
        {
            if (entity.Velocity.HorizontalLengthSquared() <= 0.08 * 0.08)
            {
                break;
            }
            
            SetHorizontalMovementsFromVector(
                entity.Velocity.Scaled(-1),
                entity.Yaw,
                physics.InputControls);
               
            await physics.WaitForTick();  
        }
        physics.InputControls.Reset();
    }

    private static void RotateVector(MutableVector3 vec, double yaw)
    {
        var sin = Math.Sin(yaw * (Math.PI / 180.0));
        var cos = Math.Cos(yaw * (Math.PI / 180.0));

        var x = vec.X * cos - vec.Z * sin;
        var z = vec.Z * cos + vec.X * sin;
        
        if (Math.Abs(x) < 0.02)
            x = 0;
        if (Math.Abs(z) < 0.02)
            z = 0;

        vec.Set(x, vec.Y, z);
    }

    public static Vector3 GetPositionNextTick(Entity entity)
    {
        return entity.Position.Plus(entity.Velocity);
    }
    
    public static Vector3 GetXZPositionNextTick(Entity entity)
    {
        return entity.Position.Clone().Add(entity.Velocity.X, 0, entity.Velocity.Z);
    }

    /// <summary>
    /// Makes sure the whole xz-hitbox of <paramref name="entity"/> is inside the block at <paramref name="blockPosition"/>
    /// </summary>
    public static async Task MoveInsideBlock(Entity entity, Position blockPosition, PhysicsPlugin physics)
    {
        // assert entity's bb intersects block position
        var bb = entity.GetBoundingBox();
        
        if (!CollisionHelper.IntersectsBbWithBlock(bb, blockPosition))
        {
            throw new MoveWentWrongException($"Cannot move to block center of {blockPosition}, because entity is at {entity.Position}");
        }
        
        var toTarget = new MutableVector3(0.5, 0, 0.5).Add(blockPosition);
        
        while (true)
        {
            var vec = toTarget.Minus(entity.Position);
            SetHorizontalMovementsFromVector(vec, entity.Yaw, physics.InputControls);
            await physics.WaitForTick();

            CollisionHelper.SetAabbToPlayerBB(GetXZPositionNextTick(entity), bb);
            
            if (CollisionHelper.IsPointInBlockBb(bb.Min, blockPosition) 
             && CollisionHelper.IsXzPointInBlockBb(bb.Max, blockPosition))
            {
                break;
            } 
        }
        
        physics.InputControls.Reset();
    }
}

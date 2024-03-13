using MineSharp.Bot.Plugins;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Entities;
using MineSharp.Core.Geometry;
using MineSharp.Pathfinder.Exceptions;
using MineSharp.Physics.Input;

namespace MineSharp.Pathfinder.Utils;

internal static class MovementUtils
{
    private const double DISTANCE_THRESHOLD = 0.15 * 0.15;

    public static bool IsOnSameBlock(MinecraftPlayer player, Position block)
    {
        return (int)player.Entity!.Position.X == block.X
            && (int)player.Entity!.Position.Y == block.Y
            && (int)player.Entity!.Position.Z == block.Z;
    }
    
    public static void SetHorizontalMovementsFromVector(Vector3 movement, InputControls controls)
    {
        controls.Reset();
        
        if (movement.Z > 0)
            controls.ForwardKeyDown = true;

        if (movement.Z < 0)
            controls.BackwardKeyDown = true;

        if (movement.X > 0)
            controls.LeftKeyDown = true;

        if (movement.X < 0)
            controls.RightKeyDown = true;
    }

    public static async Task SlowDown(Entity entity, PhysicsPlugin physics)
    {
        SetHorizontalMovementsFromVector(
            entity.Velocity.Scaled(-1), physics.InputControls);
        
        await physics.WaitForTick();
        physics.InputControls.Reset();
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
        
        var toTarget = new Vector3(0.5, 0, 0.5).Add(blockPosition);
        
        while (true)
        {
            var vec = toTarget.Minus(entity.Position);
            SetHorizontalMovementsFromVector(vec, physics.InputControls);
            await physics.WaitForTick();

            CollisionHelper.SetAABBToPlayerBB(GetXZPositionNextTick(entity), ref bb);
            
            if (CollisionHelper.IsPointInBlockBb(bb.Min, blockPosition) 
             && CollisionHelper.IsXzPointInBlockBb(bb.Max, blockPosition))
            {
                break;
            } 
        }
        
        physics.InputControls.Reset();
    }
}

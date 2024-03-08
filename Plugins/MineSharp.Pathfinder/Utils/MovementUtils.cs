using MineSharp.Bot.Plugins;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Entities;
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

    public static Vector3 GetXZPositionNextTick(Entity entity)
    {
        return entity.Position.Clone().Add(entity.Velocity.X, 0, entity.Velocity.Z);
    }

    public static async Task MoveToBlockCenter(MinecraftPlayer player, PhysicsPlugin physics)
    {
        var target   = (Position)player.Entity!.Position;
        var toTarget = new Vector3(0.5, 0, 0.5).Add(target);
        
        var bb = CollisionHelper.SetAABBToPlayerBB(player.Entity!.Position);
        
        while (true)
        {
            var vec = toTarget.Minus(player.Entity.Position);
            SetHorizontalMovementsFromVector(vec, physics.InputControls);
            await physics.WaitForTick();

            CollisionHelper.SetAABBToPlayerBB(GetXZPositionNextTick(player.Entity), ref bb);
            
            if (CollisionHelper.IsPositionInBlock(bb.Min, target) 
             && CollisionHelper.IsXZPositionInBlock(bb.Max, target))
            {
                break;
            } 
        }
        
        physics.InputControls.Reset();
    }
}

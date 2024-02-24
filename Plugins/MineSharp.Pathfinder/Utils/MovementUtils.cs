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

    public static async Task MoveToBlockCenter(MinecraftPlayer player, PhysicsPlugin physics)
    {
        var target = player.Entity!.Position
                           .Floored()
                           .Add(0.5, 0.0, 0.5);

        while (true)
        {
            var vec = target.Minus(player.Entity.Position);
            SetHorizontalMovementsFromVector(vec, physics.InputControls);

            await physics.WaitForTick();

            var dst = player.Entity.Position
                            .Plus(player.Entity.Velocity)
                            .HorizontalDistanceToSquared(target);
            
            Console.WriteLine($"Dst = {dst}");
            
            if (dst < DISTANCE_THRESHOLD)
                break;
        }
        
        physics.InputControls.Reset();
    }
}

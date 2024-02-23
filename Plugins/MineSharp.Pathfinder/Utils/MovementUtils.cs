using MineSharp.Bot.Plugins;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Entities;
using MineSharp.Physics.Input;

namespace MineSharp.Pathfinder.Utils;

internal static class MovementUtils
{
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
            
            Console.WriteLine($"Vec = {vec} "                    +
                              $"Pos = {player.Entity.Position} " +
                              $"Vel = {player.Entity.Velocity} " +
                              $"Trg = {target}" +
                              $"Dst = {dst}"); 
            
            if (dst < 0.1 * 0.1)
                break;
        }
        
        physics.InputControls.Reset();
    }
}

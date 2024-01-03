using MineSharp.Bot;
using MineSharp.Bot.Plugins;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Pathfinder.Utils;
using MineSharp.World;

namespace MineSharp.Pathfinder.Moves;

public class DirectMove(Vector3 motion) : IMove
{
    /// <inheritdoc />
    public Vector3 Motion { get; } = motion;
    
    /// <inheritdoc />
    public float Cost => 1;

    /// <inheritdoc />
    public bool CanBeLinked => true;

    /// <inheritdoc />
    public bool IsMovePossible(Vector3 position, IWorld world, MinecraftData data)
    {
        return CollisionHelper.HasBlockSpaceForStanding(
            position.Plus(this.Motion), 
            world, 
            data);
    }
    
    /// <inheritdoc />
    public Task DoTick(PlayerPlugin playerPlugin, PhysicsPlugin physics, int count, Movements movements)
    {
        physics.InputControls.SprintingKeyDown = movements.AllowSprinting;
        physics.InputControls.ForwardKeyDown = true;

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task StartMove(PlayerPlugin playerPlugin, PhysicsPlugin physics, int count, Movements movements)
    {
        var target = playerPlugin.Self!.Entity!.Position.Plus(this.Motion.Scaled(count));
        
        var normalized = this.Motion.Normalized();
        var yaw = (180 / MathF.PI) * (float)Math.Atan2(-normalized.X, normalized.Z);

        physics.InputControls.Reset();
        await physics.Look(yaw, playerPlugin.Self!.Entity!.Pitch);
    }
}
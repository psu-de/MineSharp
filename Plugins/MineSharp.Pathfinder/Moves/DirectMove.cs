using MineSharp.Bot;
using MineSharp.Bot.Plugins;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Pathfinder.Utils;
using MineSharp.World;
using NLog;

namespace MineSharp.Pathfinder.Moves;

public class DirectMove(Vector3 motion) : IMove
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); 
    
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
    public async Task PerformMove(MineSharpBot bot, int count, Movements movements)
    {
        var player = bot.GetPlugin<PlayerPlugin>();
        var physics = bot.GetPlugin<PhysicsPlugin>();
        
        physics.InputControls.Reset();

        var target = player.Self!.Entity!.Position
            .Floored()
            .Add(0.5, 0.0, 0.5)
            .Add(this.Motion.Scaled(count));
        
        await physics.LookAt(target);
        var forward = true;
        var sprint = movements.AllowSprinting;
        var hasStoppedSprinting = !movements.AllowSprinting;
        var prevDst = double.MaxValue;
        
        while (true)
        {
            physics.InputControls.ForwardKeyDown = forward;
            physics.InputControls.SprintingKeyDown = sprint;
            await physics.WaitForTick();

            if (!forward)
            {
                // adjust rotation when close to target
                await physics.LookAt(target);
                forward = true;
            }

            var dst = target.DistanceToSquared(player.Self!.Entity!.Position);
            if (dst > prevDst)
            {
                Logger.Warn("Getting further away from target!");
                throw new Exception(); // TODO: Better exception
            }
            
            if (dst <= 0.0625)
                break;

            if (hasStoppedSprinting || dst > 0.5)
                continue;

            // stop walking for one tick to stop sprinting
            // bot could move too far if it is too fast
            forward = false;
            sprint = false;
            hasStoppedSprinting = true;
        }
        
        physics.InputControls.Reset();
    }
}
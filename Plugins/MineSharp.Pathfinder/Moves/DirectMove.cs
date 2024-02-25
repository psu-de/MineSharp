using MineSharp.Bot;
using MineSharp.Bot.Plugins;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Pathfinder.Utils;
using MineSharp.World;
using MineSharp.World.Iterators;
using NLog;

namespace MineSharp.Pathfinder.Moves;

/// <summary>
/// Move to a directly adjacent block
/// </summary>
/// <param name="motion"></param>
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
    public bool IsMovePossible(Position position, IWorld world)
    {
        var playerBb = CollisionHelper.GetPlayerBoundingBox(position);
        playerBb.Offset(
            0.5 + this.Motion.X / 2, 
            0, 
            0.5 + this.Motion.Z / 2);

        return !CollisionHelper.CollidesWithWord(playerBb, world);
    }

    /// <inheritdoc />
    public async Task PerformMove(MineSharpBot bot, int count, Movements movements)
    { 
        // TODO: IsMovePossible() ist nicht korrekt
        var player = bot.GetPlugin<PlayerPlugin>();
        var physics = bot.GetPlugin<PhysicsPlugin>();
        var entity  = player.Entity ?? throw new NullReferenceException("Player entity not initialized.");

        physics.InputControls.Reset();
        
        await physics.Look(0, 0);
        await MovementUtils.MoveToBlockCenter(player.Self!, physics);
        
        var target = player.Self!.Entity!.Position
            .Floored()
            .Add(0.5, 0.0, 0.5)
            .Add(this.Motion.Scaled(count));

        var targetBlock = (Position)target;
        
        MovementUtils.SetHorizontalMovementsFromVector(
            this.Motion, physics.InputControls);

        while (true)
        {
            await physics.WaitForTick();

            if (CollisionHelper.IsOnPosition(entity, targetBlock))
                break;
        }
        
        physics.InputControls.Reset();
        await MovementUtils.MoveToBlockCenter(player.Self, physics);
    }
}

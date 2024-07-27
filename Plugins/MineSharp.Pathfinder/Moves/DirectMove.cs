using MineSharp.Bot;
using MineSharp.Bot.Plugins;
using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
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
public class DirectMove(Vector3 motion) : Move
{
    /// <inheritdoc />
    public override Vector3 Motion { get; } = motion;
    
    /// <inheritdoc />
    public override float Cost => 1;

    /// <inheritdoc />
    public override bool CanBeLinked => true;

    /// <inheritdoc />
    public override bool IsMovePossible(Position position, IWorld world, MinecraftData data)
    {
        var playerBb = CollisionHelper.GetAabbForPlayer(position);
        playerBb.Offset(
            0.5 + this.Motion.X / 2, 
            0, 
            0.5 + this.Motion.Z / 2);

        return !CollisionHelper.CollidesWithWord(playerBb, world, data);
    }

    /// <inheritdoc />
    protected override async Task PerformMove(MineSharpBot bot, int count, Movements movements)
    { 
        var player  = bot.GetPlugin<PlayerPlugin>();
        var physics = bot.GetPlugin<PhysicsPlugin>();
        var entity  = player.Entity ?? throw new NullReferenceException("Player entity not initialized.");
        
        var target = player.Self!.Entity!.Position
            .Floored()
            .Add(0.5, 0.0, 0.5)
            .Add(this.Motion.Scaled(count));

        var targetBlock = (Position)target;
        
        MovementUtils.SetHorizontalMovementsFromVector(
            this.Motion, entity.Yaw, physics.InputControls);
        physics.InputControls.SprintingKeyDown = movements.AllowSprinting;

        while (true)
        {
            await physics.WaitForTick();
            
            if (CollisionHelper.IsPointInBlockBb(entity.Position, targetBlock))
            {
                break;
            }
        }
        physics.InputControls.Reset();
    }
}

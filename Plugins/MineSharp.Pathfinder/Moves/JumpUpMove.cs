using MineSharp.Bot;
using MineSharp.Bot.Plugins;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Pathfinder.Utils;
using MineSharp.World;
using MineSharp.World.Iterators;

namespace MineSharp.Pathfinder.Moves;

/// <summary>
/// Jump up to an adjacent block
/// </summary>
/// <param name="xzMotion"></param>
public class JumpUpMove(Vector3 xzMotion) : IMove
{
    /// <inheritdoc />
    public Vector3 Motion { get; } = Vector3.Up.Plus(xzMotion);
    
    /// <inheritdoc />
    public float Cost => 20;

    /// <inheritdoc />
    public bool CanBeLinked => false;

    /// <inheritdoc />
    public bool IsMovePossible(Position position, IWorld world)
    {
        var playerBb = CollisionHelper.SetAABBToPlayerBB(position);
        playerBb.Offset(
            0.5 + this.Motion.X / 2, 
            1, 
            0.5 + this.Motion.Z / 2);

        return !CollisionHelper.CollidesWithWord(playerBb, world);
    }


    /// <inheritdoc />
    public async Task PerformMove(MineSharpBot bot, int count, Movements movements)
    {
        if (count != 1)
            throw new InvalidOperationException();

        var physics = bot.GetPlugin<PhysicsPlugin>();
        var player = bot.GetPlugin<PlayerPlugin>();

        var target = player.Self!.Entity!.Position
            .Floored()
            .Add(this.Motion)
            .Add(0.5, 0.0, 0.5);

        var targetBlock = (Position)target;

        await physics.Look(0, 0);
        
        MovementUtils.SetHorizontalMovementsFromVector(this.Motion, physics.InputControls);
        physics.InputControls.JumpingKeyDown = true;
        
        while (true)
        {
            await physics.WaitForTick();

            if ((int)player.Entity!.Position.X == targetBlock.X
             && (int)player.Entity!.Position.Z == targetBlock.Z)
            {
                break;
            }
        }
        
        physics.InputControls.Reset();
        await physics.WaitForOnGround();

        await MovementUtils.MoveToBlockCenter(player.Self, physics);
    }
}

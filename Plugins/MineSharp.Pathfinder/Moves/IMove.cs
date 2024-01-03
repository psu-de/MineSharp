using MineSharp.Core.Common;
using MineSharp.Bot;
using MineSharp.Bot.Plugins;
using MineSharp.Core.Common.Entities;
using MineSharp.Data;
using MineSharp.Physics.Input;
using MineSharp.World;

namespace MineSharp.Pathfinder.Moves;

/// <summary>
/// Interface for creating a move
/// </summary>
public interface IMove
{
    /// <summary>
    /// Relative movement vector for this move
    /// </summary>
    public Vector3 Motion { get; }
    
    /// <summary>
    /// Cost for A*. A higher cost means the move will be performed less likely.
    /// </summary>
    public float Cost { get; }

    /// <summary>
    /// Checks if the move is possible
    /// </summary>
    public bool IsMovePossible(Vector3 position, IWorld world, MinecraftData data);

    /// <summary>
    /// Whether multiple instances of this move can be
    /// joint together.
    /// </summary>
    public bool CanBeLinked { get; }
    
    /// <summary>
    /// Perform a single move tick.
    /// </summary>
    /// <param name="playerPlugin"></param>
    /// <param name="physics"></param>
    /// <param name="count"></param>
    /// <param name="movements"></param>
    /// <returns></returns>
    internal Task DoTick(PlayerPlugin playerPlugin, PhysicsPlugin physics, int count, Movements movements);

    /// <summary>
    /// Called once before performing the move
    /// </summary>
    /// <param name="playerPlugin"></param>
    /// <param name="physics"></param>
    /// <param name="count"></param>
    /// <param name="movements"></param>
    /// <returns></returns>
    internal Task StartMove(PlayerPlugin playerPlugin, PhysicsPlugin physics, int count, Movements movements)
        => Task.CompletedTask;

    /// <summary>
    /// Called once after the move has been performed
    /// </summary>
    /// <param name="playerPlugin"></param>
    /// <param name="physics"></param>
    /// <param name="count"></param>
    /// <param name="movements"></param>
    /// <returns></returns>
    internal Task StopMove(PlayerPlugin playerPlugin, PhysicsPlugin physics, int count, Movements movements)
        => Task.CompletedTask;
}
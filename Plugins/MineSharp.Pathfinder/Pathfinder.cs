using MineSharp.Bot;
using MineSharp.Bot.Plugins;
using MineSharp.Core.Common;
using MineSharp.Pathfinder.Algorithm;
using MineSharp.Pathfinder.Moves;
using NLog;

namespace MineSharp.Pathfinder;

/// <summary>
/// The Pathfinder plugin provides methods to navigate through the
/// Minecraft World.
/// </summary>
public class Pathfinder(MineSharpBot bot) : Plugin(bot)
{
    private static ILogger Logger = LogManager.GetCurrentClassLogger();
    
    private AStar? astar;
    private PhysicsPlugin? physics;
    private WorldPlugin? worldPlugin;
    private PlayerPlugin? playerPlugin;
    private Movements movements = Movements.Default;

    protected override Task Init()
    {
        this.physics = this.Bot.GetPlugin<PhysicsPlugin>();
        this.worldPlugin = this.Bot.GetPlugin<WorldPlugin>();
        this.playerPlugin = this.Bot.GetPlugin<PlayerPlugin>();

        this.astar = new AStar(this.worldPlugin.World, this.Bot.Data, this.movements);

        return Task.WhenAll(
            this.physics.WaitForInitialization(), 
            this.worldPlugin.WaitForInitialization(),
            this.playerPlugin.WaitForInitialization());
    }

    public async Task Goto(Position position)
    {
        var start = this.playerPlugin!.Self!.Entity!.Position;
        var path = this.astar!.FindPath((Position)start, position);
        
        Logger.Debug("Found path with {count} nodes", path.Nodes.Length);
        foreach (var node in path.Nodes)
        {
            Logger.Debug("Move: {count} * {motion} - {moveType}", node.Count, node.Move.Motion, node.Move.GetType());

            await this.PerformMove(node.Move, node.Count);
        }

        return;
    }

    /// <summary>
    /// Do the move
    /// </summary>
    /// <param name="move"></param>
    /// <param name="count"></param>
    public Task PerformMove(IMove move, int count = 1)
    {
        return move.PerformMove(this.Bot, count, this.movements);
    }
}
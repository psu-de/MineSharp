using MineSharp.Bot;
using MineSharp.Bot.Plugins;
using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
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

        this.astar = new AStar(this.worldPlugin.World, this.movements, this.Bot.Data);

        return Task.WhenAll(
            this.physics.WaitForInitialization(), 
            this.worldPlugin.WaitForInitialization(),
            this.playerPlugin.WaitForInitialization());
    }

    public async Task Goto(Position position)
    {
        var start = this.playerPlugin!.Self!.Entity!.Position;
        var path = this.astar!.FindPath((Position)start, position);
        
        foreach (var node in path.Nodes)
        {
            await node.Move.Perform(this.Bot, node.Count, (Position)node.Position, this.movements);
        }

        return;
    }
}

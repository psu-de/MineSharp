using MineSharp.Bot;
using MineSharp.Bot.Plugins;
using MineSharp.Core.Common;
using MineSharp.Pathfinder.Algorithm;
using MineSharp.Pathfinder.Moves;

namespace MineSharp.Pathfinder;

/// <summary>
/// The Pathfinder plugin provides methods to navigate through the
/// Minecraft World.
/// </summary>
public class Pathfinder(MineSharpBot bot) : Plugin(bot)
{
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
        
        Console.WriteLine($"Found path with {path.Nodes.Length} nodes");
        foreach (var node in path.Nodes)
        {
            Console.WriteLine($"Move: {node.Count} * {node.Move.Motion} - {node.Move.GetType()}");

            await this.PerformMove(node.Move, node.Count);
        }

        return;
    }

    /// <summary>
    /// Do the move
    /// </summary>
    /// <param name="move"></param>
    /// <param name="count"></param>
    public async Task PerformMove(IMove move, int count = 1)
    {
        var target = this.playerPlugin!.Entity!.Position.Plus(
            move.Motion.Scaled(count));
        
        var tsc = new TaskCompletionSource();

        void PhysicsTick(MineSharpBot bot)
        {
            move.DoTick(this.playerPlugin!, this.physics!, count, this.movements)
                .Wait();
            
            var dst = playerPlugin.Self!.Entity!.Position
                .DistanceToSquared(target);
            
            if (dst <= 0.223)
                tsc.SetResult();
        }
        
        await move.StartMove(this.playerPlugin!,
            this.physics!,
            count,
            this.movements);

        this.physics!.PhysicsTick += PhysicsTick;

        await tsc.Task;

        this.physics!.PhysicsTick -= PhysicsTick;
        
        physics.InputControls.Reset();

        await move.StopMove(
            this.playerPlugin!,
            this.physics,
            count,
            this.movements);
    }
}
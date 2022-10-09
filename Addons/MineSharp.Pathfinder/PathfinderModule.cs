using MineSharp.Bot;
using MineSharp.Bot.Modules;
using MineSharp.Pathfinding.Algorithm;
using MineSharp.Pathfinding.Goals;

namespace MineSharp.Pathfinding
{
    public class PathfinderModule : Module
    {
        public PathfinderModule(MinecraftBot bot) : base(bot) {}

        protected override Task Load() => Task.CompletedTask;

        public async Task<Path> FindPath(Goal goal, Movements? movements = null, double? timeout = null, CancellationToken? cancellation = null)
        {
            cancellation ??= CancellationToken.None;
            movements ??= new Movements();
            
            if (this.Bot.World == null)
            {
                await this.Bot.WaitForChunksToLoad();
            }

            if (this.Bot.Player == null)
            {
                await this.Bot.WaitForBot();
            }
            
            var astar = new AStar(this.Bot.Player!, this.Bot.World!, movements);
            var path = await Task.Run(() => astar.ComputePath(goal, timeout)).WaitAsync(cancellation.Value);
            return path;
        }

        public async Task MovePath(Path path, CancellationToken? cancellation = null)
        {
            cancellation ??= CancellationToken.None;
            await path.MoveAlong(this.Bot, cancellation);
            await this.Bot.PlayerControls.Reset();
        }
        
        public async Task GoTo(Goal goal, Movements? movements = null, double? timeout = null, CancellationToken? cancellation = null)
        {
            cancellation = cancellation ?? CancellationToken.None;
            movements = movements ?? new Movements();

            var path = await this.FindPath(goal, movements, timeout, cancellation);

            await this.MovePath(path);
        }
    }
}

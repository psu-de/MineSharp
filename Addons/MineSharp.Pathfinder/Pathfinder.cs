using MineSharp.Bot;
using MineSharp.Bot.Modules;
using MineSharp.Core.Types;
using MineSharp.Pathfinding.Algorithm;
using MineSharp.Pathfinding.Goals;
using MineSharp.Pathfinding.Moves;

namespace MineSharp.Pathfinding
{
    public class Pathfinder : Module
    {
        public Pathfinder(MinecraftBot bot) : base(bot)
        {
        }

        protected override Task Load()
        {
            return Task.CompletedTask;
        }

        public async Task GoTo(Goal goal, Movements? movements = null, double? timeout = null)
        {

            movements = movements ?? new Movements();
            if (this.Bot.World == null)
            {
                await this.Bot.WaitForChunksToLoad();
            }
            if (this.Bot.Player == null)
            {
                await this.Bot.WaitForBot();
            }

            var astar = new AStar(this.Bot.Player!, this.Bot.World!, movements);
            var path = astar.ComputePath(goal, timeout);
            var pos = this.Bot.BotEntity!.Position.Clone();

            var moves = new List<Move>();
            for (int i = 1; i < path.Length; i++)
            {
                var nodeFrom = path[i - 1];
                var nodeTo = path[i];

                var diff = nodeTo.Position.Minus(nodeFrom.Position);
                var move = movements.GetMoveByVector(diff);
                moves.Add(move);
            }
            Logger.Debug($"Found {moves.Count} moves");

            foreach (var move in moves)
            {
                await move.PerformMove(this.Bot);
            }

            await this.Bot.PlayerControls.Reset();
        }
    }
}
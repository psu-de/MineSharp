using MineSharp.Bot;
using MineSharp.Bot.Modules;
using MineSharp.Core.Types;
using MineSharp.Pathfinding.Algorithm;
using MineSharp.Pathfinding.Goals;

namespace MineSharp.Pathfinding
{
    public class Pathfinder : Module
    {
        public Movements DefaultMovements { get; set; }

        private List<(Vector3, TaskCompletionSource)> AwaitedPositions = new List<(Vector3, TaskCompletionSource)>();

        public Pathfinder(MinecraftBot bot, Movements? movements = null) : base(bot)
        {
            this.DefaultMovements = movements ?? Movements.DefaultMovements;
        }

        protected override Task Load()
        {
            this.Bot.BotMoved += Bot_BotMoved;
            return Task.CompletedTask;
        }

        private void Bot_BotMoved(MinecraftBot sender, Core.Types.MinecraftPlayer entity)
        {
            List<int> toRemove = new List<int>();
            for (int i = 0; i < AwaitedPositions.Count; i++)
            {
                (var pos, var tsc) = AwaitedPositions[i];
                if (pos.DistanceSquared(entity.Entity.Position) < 0.5f)
                {
                    tsc.SetResult();
                    toRemove.Add(i);
                }
            }
            toRemove.ForEach(i => AwaitedPositions.RemoveAt(i));
        }

        internal Task WaitUntilReached(Vector3 pos)
        {
            var tsc = new TaskCompletionSource();
            AwaitedPositions.Add((pos, tsc));
            return tsc.Task;
        }

        public async Task GoTo(Goal goal)
        {
            if (this.Bot.World == null)
            {
                await this.Bot.WaitForChunksToLoad();
            }
            if (this.Bot.Player == null)
            {
                await this.Bot.WaitForBot();
            }

            var astar = new AStar(this.Bot.Player!, this.Bot.World!);
            var path = astar.ComputePath(goal);


            foreach (var node in path)
            {
                var diff = node.Position.Minus(this.Bot.BotEntity!.Position.Floored());
                Logger.Debug("Going to " + node + " diff: " + diff);

                if (diff == Vector3.South)
                {
                    this.Bot.ForceSetRotation(0, 0);
                } else if (diff == Vector3.West)
                {
                    this.Bot.ForceSetRotation(90, 0);
                } else if (diff == Vector3.North)
                {
                    this.Bot.ForceSetRotation(180, 0);
                } else if (diff == Vector3.East)
                {
                    this.Bot.ForceSetRotation(-90, 0);
                }

                this.Bot.PlayerControls.Walk(MineSharp.Bot.Enums.WalkDirection.Forward);
                await WaitUntilReached(node.Position);
                this.Bot.PlayerControls.StopWalk(MineSharp.Bot.Enums.WalkDirection.Forward);
            }
        }

        ~Pathfinder()
        {
            this.Bot.BotMoved -= Bot_BotMoved;
        }

    }
}
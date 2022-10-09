using MineSharp.Bot;
using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using MineSharp.Pathfinding.Algorithm;
using MineSharp.Pathfinding.Moves;

namespace MineSharp.Pathfinding
{
    public class Path
    {
        private static readonly Logger Logger = Logger.GetLogger();

        /// <summary>
        ///     Array of Nodes of the path
        /// </summary>
        public Node[] Nodes { get; }
        
        /// <summary>
        ///     List of indexes in Nodes when the path turns or
        ///     another Move comes next
        /// </summary>
        public List<int> WaypointIndexes { get; private set; }
        
        /// <summary>
        ///     List of the simplified moves
        /// </summary>
        public List<(Move Move, Vector3 Position, int Count)> SimplifiedMoves { get; private set; }
        
        public Path(Node[] nodes)
        {
            if (nodes[0].Move != null || nodes.Skip(1).Any(x => x.Move == null))
            {
                throw new Exception("Invalid array of nodes");
            }
            
            this.Nodes = nodes;
            this.SimplifiedMoves = new List<(Move, Vector3, int)>();
            this.WaypointIndexes = new List<int>();

            this.SimplifyPath();
        }

        private void SimplifyPath()
        {
            this.SimplifiedMoves = new List<(Move, Vector3, int)>();
            this.WaypointIndexes = new List<int>();

            var lastMove = this.Nodes[1].Move!;
            this.SimplifiedMoves.Add((lastMove, this.Nodes[0].Position, 1));
            
            for (int i = 2; i < this.Nodes.Length; i++)
            {
                var currentNode = this.Nodes[i];
                var currentMove = currentNode.Move!;
                
                if (currentMove.CanBeSimplified &&
                    lastMove.GetType() == currentMove.GetType() &&
                    lastMove.MoveVector == currentMove.MoveVector)
                {
                    var move = this.SimplifiedMoves[^1];
                    move.Count++;
                    this.SimplifiedMoves[^1] = move;
                } else
                {
                    this.WaypointIndexes.Add(i);
                    this.SimplifiedMoves.Add((currentMove, this.Nodes[i - 1].Position, 1));
                    lastMove = currentMove;
                }
            }

            Logger.Debug($"Simplified to {SimplifiedMoves.Count} moves");
            Logger.Debug($"Found {WaypointIndexes.Count} waypoints");
        }

        public async Task MoveAlong(MinecraftBot bot, CancellationToken? cancellationToken = null)
        {
            cancellationToken = cancellationToken ?? CancellationToken.None;

            await bot.PlayerControls.Reset();

            foreach ((var move, var position, var count) in this.SimplifiedMoves)
            {
                if (cancellationToken.Value.IsCancellationRequested)
                {
                    return;
                }
                
                await move.PerformMove(bot, position, count,  cancellationToken);
            }
        }
    }
}

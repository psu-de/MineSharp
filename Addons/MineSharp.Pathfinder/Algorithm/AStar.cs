using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using MineSharp.Data.Blocks;
using MineSharp.Pathfinding.Goals;
using MineSharp.Pathfinding.Moves;
using MineSharp.Physics;
using MineSharp.World;
using System.Reflection.Metadata;
using Priority_Queue;

namespace MineSharp.Pathfinding.Algorithm
{
    public class AStar
    {
        private static readonly Logger Logger = Logger.GetLogger();
        public const int DEFAULT_MAX_NODES = 5000;

        public MinecraftPlayer Player { get; set; }
        public World.World World { get; set; }
        public Movements Movements { get; set; }

        public AStar(MinecraftPlayer player, World.World world, Movements? movements = null)
        {
            this.Player = player;
            this.World = world;
            this.Movements = movements ?? new Movements();
        }

        public Node[] ComputePath(Goal goal, double? timeout = null, int maxNodes = DEFAULT_MAX_NODES)
        {
            var startTime = DateTime.Now;

            var openSet = new FastPriorityQueue<Node>(maxNodes);
            var closedSet = new HashSet<Node>();
            var nodes = new Dictionary<ulong, Node>();

            var pos = this.Player.Entity.Position.Floored();

            using (_ = new TemporaryBlockCache(this.World))
            {
                var startNode = this.GetNodeForBlock(pos, ref nodes);
                var endNode = this.GetNodeForBlock(goal.Target, ref nodes);

                if (!endNode.Walkable)
                {
                    throw new Exception($"Target block is not walkable");
                }

                openSet.Enqueue(startNode, startNode.fCost);

                while (openSet.Count > 0)
                {
                    if (timeout != null && (DateTime.Now - startTime).TotalMilliseconds > timeout)
                    {
                        throw new Exception($"Could not find a path after {Math.Round((DateTime.Now - startTime).TotalMilliseconds * 10) / 10}ms");
                    }

                    var node = openSet.Dequeue();
                    closedSet.Add(node);

                    if (node.Position == goal.Target.Floored())
                    {
                        Logger.Debug($"Checked {nodes.Count} nodes in {Math.Round((DateTime.Now - startTime).TotalMilliseconds * 10) / 10}ms");
                        var path = new List<Node>();
                        var currentNode = node;

                        while (currentNode != startNode)
                        {
                            path.Add(currentNode!);
                            currentNode = currentNode!.Parent!;
                        }
                        path.Add(startNode);
                        path.Reverse();
                        Logger.Debug($"Found Path with {path.Count} nodes");
                        return path.ToArray();
                    }

                    var neighbors = this.GetNeighbors(node, ref nodes).ToArray();

                    foreach ((var neighbor, var move) in neighbors)
                    {
                        if (!neighbor.Walkable || closedSet.Contains(neighbor))
                        {
                            continue;
                        }

                        var newCost = node.gCost + (float)goal.Target.Distance(neighbor.Position) + move.MoveCost;
                        if (newCost < neighbor.gCost || !openSet.Contains(node))
                        {
                            neighbor.gCost = newCost;
                            neighbor.hCost = (float)goal.Target.Distance(neighbor.Position) + move.MoveCost;
                            neighbor.Parent = node;
                            neighbor.Move = move;

                            if (!openSet.Contains(neighbor))
                            {
                                openSet.Enqueue(neighbor, neighbor.fCost);
                            } else
                            {
                                openSet.UpdatePriority(neighbor, neighbor.fCost);
                            }
                        }
                    }
                }
            }

            throw new Exception("No path found");
        }

        private List<(Node, Move)> GetNeighbors(Node node, ref Dictionary<ulong, Node> nodes)
        {
            var neighbors = new List<(Node, Move)>();

            foreach (var move in this.Movements.PossibleMoves)
            {
                if (move.IsMovePossible(node.Position, this.World))
                {
                    var pos = node.Position.Plus(move.MoveVector);
                    if (!this.World.IsBlockLoaded(pos))
                    {
                        continue;
                    }
                    var neighborNode = this.GetNodeForBlock(pos, ref nodes);
                    neighbors.Add((neighborNode, move));
                }
            }

            return neighbors;
        }

        private Node GetNodeForBlock(Vector3 pos, ref Dictionary<ulong, Node> nodes)
        {
            pos = pos.Floored();
            if (nodes.TryGetValue(((Position)pos).ToULong(), out var node))
            {
                return node;
            }

            var block = this.World.GetBlockAt(pos);
            var walkable = !(block.Id == Water.BlockId || PhysicsConst.WaterLikeBlocks.Contains(block.Id));

            var newNode = new Node(pos, walkable, 0, 0);
            nodes.Add(((Position)pos).ToULong(), newNode);
            return newNode;
        }
    }
}

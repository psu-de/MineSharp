using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using MineSharp.Data.Blocks;
using MineSharp.Pathfinding.Goals;
using Org.BouncyCastle.Asn1;
using Priority_Queue;
using Spectre.Console;

namespace MineSharp.Pathfinding.Algorithm
{
    public class AStar
    {
        static readonly Logger Logger = Logger.GetLogger();
        public const int DEFAULT_MAX_NODES = 1000;

        public MinecraftPlayer Player { get; set; }
        public World.World World { get; set; }

        public AStar(MinecraftPlayer player, World.World world)
        {
            this.Player = player;
            this.World = world;
        }

        public Node[] ComputePath(Goal goal)
        {
            List<Node> openSet = new();
            HashSet<Node> closedSet = new HashSet<Node>();
            Dictionary<ulong, Node> nodes = new Dictionary<ulong, Node>();

            var pos = Player.Entity.Position.Clone();

            var startNode = GetNodeForBlock(pos, ref nodes);
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                var node = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
                    {
                        if (openSet[i].hCost < node.hCost)
                        {
                            node = openSet[i];
                        }
                    }
                }
                Logger.Debug($"Checking {node}");

                openSet.Remove(node);
                closedSet.Add(node);

                Logger.Debug($"{node.Position} == {goal.Target.Floored()}: {node.Position == goal.Target.Floored()}");
                if (node.Position == goal.Target.Floored())
                {
                    List<Node> path = new List<Node>();
                    Node currentNode = node;

                    while (currentNode != startNode)
                    {
                        path.Add(currentNode!);
                        currentNode = currentNode!.Parent!;
                    }
                    path.Reverse();
                    return path.ToArray();
                }

                var neighbors = GetNeighbors(node, ref nodes).ToArray();
                Logger.Debug($"Found {neighbors.Length} neighbors");

                foreach (var neighbor in neighbors)
                {
                    if (!neighbor.Walkable || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    var newCost = node.gCost + goal.Target.DistanceSquared(neighbor.Position);
                    if (newCost < neighbor.gCost || !openSet.Contains(node))
                    {
                        neighbor.gCost = newCost;
                        neighbor.hCost = goal.Target.DistanceSquared(neighbor.Position);
                        neighbor.Parent = node;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }
            throw new Exception("No path found");
        }

        private List<Node> GetNeighbors(Node node, ref Dictionary<ulong, Node> nodes)
        {
            var offsets = new Vector3[] {
                Vector3.North, Vector3.East, Vector3.South, Vector3.West
            };

            List<Node> neighbors = new List<Node>();    

            foreach (var offset in offsets)
            {
                var pos = node.Position.Plus(offset);
                neighbors.Add(GetNodeForBlock(pos, ref nodes));
            }

            return neighbors;
        }

        private Node GetNodeForBlock(Vector3 pos, ref Dictionary<ulong, Node> nodes)
        {
            pos = pos.Floored();
            if (nodes.TryGetValue(((Position)pos).ToULong(), out var node))
            {
                Logger.Debug("Reusing node");
                return node;
            }

            var block = World.GetBlockAt(pos);
            bool walkable = false;

            if (block.BoundingBox == "empty") {
                var blockAbove = World.GetBlockAt(pos.Plus(Vector3.Up));
                var blockBelow = World.GetBlockAt(pos.Plus(Vector3.Down));
                if (blockAbove.BoundingBox == "empty" && World.GetBlockAt(pos.Plus(Vector3.Down)).IsSolid())
                {
                    walkable = true;
                }
            }

            var newNode = new Node(pos, walkable, 0, 0);
            nodes.Add(((Position)pos).ToULong(), newNode);
            return newNode;
        }
    }
}

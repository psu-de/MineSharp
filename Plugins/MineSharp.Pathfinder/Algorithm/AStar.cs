using System.Diagnostics;
using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
using MineSharp.Data;
using MineSharp.Pathfinder.Exceptions;
using MineSharp.Pathfinder.Moves;
using MineSharp.World;
using NLog;
using Priority_Queue;

namespace MineSharp.Pathfinder.Algorithm;

public class AStar(IWorld world, Movements movements, MinecraftData data)
{
    public Movements Movements = movements;

    public Path FindPath(Position start, Position end, int maxNodes = 5000)
    {
        var sw        = Stopwatch.StartNew();
        var nodes     = new Dictionary<ulong, Node>();
        var openSet   = new FastPriorityQueue<Node>(maxNodes);
        var closedSet = new HashSet<Node>();

        var startNode = this.GetNode(start, end, ref nodes);
        var endNode   = this.GetNode(end, end, ref nodes);

        if (!endNode.Walkable)
            throw new InvalidOperationException("End position is not walkable!");
        
        openSet.Enqueue(startNode, startNode.FCost);

        while (openSet.Count > 0)
        {
            var node = openSet.Dequeue();

            if (node.Position == end)
            {
                sw.Stop();
                LogManager.GetCurrentClassLogger().Debug($"Checked {nodes.Count} nodes in {sw.ElapsedMilliseconds}ms");
                return RetracePath(startNode, endNode);
            }
            
            closedSet.Add(node);

            foreach (var neighbor in this.GetNeighbors(node, end, ref nodes))
            {
                if (!neighbor.Node.Walkable || closedSet.Contains(neighbor.Node))
                    continue;

                var newCost = node.GCost + this.Distance(node.Position, end) + neighbor.Move.Cost;
                if (newCost >= neighbor.Node.GCost && openSet.Contains(neighbor.Node))
                    continue;

                neighbor.Node.Parent = node;
                neighbor.Node.GCost = newCost;
                neighbor.Node.HCost = this.Distance(neighbor.Node.Position, end);
                neighbor.Node.Move = neighbor.Move;
                
                if (openSet.Contains(neighbor.Node))
                    openSet.UpdatePriority(neighbor.Node, neighbor.Node.FCost);
                else 
                    openSet.Enqueue(neighbor.Node, neighbor.Node.FCost);
            }
        }

        sw.Stop();
        LogManager.GetCurrentClassLogger().Debug($"Checked {nodes.Count} nodes in {sw.ElapsedMilliseconds}ms");
        throw new PathNotFoundException($"Could not find a path.");
    }

    private Path RetracePath(Node startNode, Node endNode)
    {
        var path = new List<Node>();
        var currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode!);
            currentNode = currentNode!.Parent!;
        }
        path.Add(startNode);
        path.Reverse();

        var moves = path.Skip(1).Select(x => x.Move!).ToArray();
        return Path.FromMoves(startNode.Position, moves);
    }

    private float Distance(Position a, Position b)
    {
        var dX = a.X - b.X;
        var dY = a.Y - b.Y;
        var dZ = a.Z - b.Z;
        return MathF.Sqrt(dX * dX + dY * dY + dZ * dZ);
    }

    private (Node Node, Move Move)[] GetNeighbors(Node node, Position end, ref Dictionary<ulong, Node> nodes)
    {
        var neighbors = new List<(Node, Move)>();
        
        foreach (var move in this.Movements.PossibleMoves)
        {
            var pos      = move.Motion.Plus(node.Position);
            var neighbor = this.GetNode((Position)pos, end, ref nodes);
            if (!neighbor.Walkable || !move.IsMovePossible(node.Position, world, data))
                continue;

            neighbors.Add((neighbor, move));
        }

        return neighbors.ToArray();
    }

    private Node GetNode(Position position, Position end, ref Dictionary<ulong, Node> nodes)
    {
        var idx = position.ToULong();
        if (nodes.TryGetValue(idx, out var node))
            return node;
        
        var block = world.GetBlockAt(position);
        var blockBelow = world.GetBlockAt((Position)Vector3.Down.Plus(position));

        var walkable = blockBelow.IsSolid() && !blockBelow.IsFluid() && !block.IsSolid(); // TODO: Waterlike blocks
        node = new Node(position, walkable, 0, this.Distance(position, end));
        nodes.Add(idx, node);

        return node;
    }
}

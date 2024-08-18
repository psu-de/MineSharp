using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
using MineSharp.Pathfinder.Moves;
using Priority_Queue;

namespace MineSharp.Pathfinder.Algorithm;

internal class Node(Position position, bool walkable, float gCost, float hCost)
    : FastPriorityQueueNode
{
    public Move? Move;
    public Node? Parent;
    
    public Position Position { get; } = position;
    public bool Walkable { get; } = walkable;

    public float GCost { get; set; } = gCost;
    public float HCost { get; set; } = hCost;
    public float FCost => this.GCost + this.HCost;
}

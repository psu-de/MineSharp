using MineSharp.Core.Types;
using MineSharp.Pathfinding.Moves;
using Priority_Queue;

namespace MineSharp.Pathfinding.Algorithm
{
    public class Node : FastPriorityQueueNode
    {

        public Move? Move;
        public Node? Parent;

        public Node(Vector3 position, bool walkable, int gCost, int hCost)
        {
            this.Position = position;
            this.Walkable = walkable;
            this.gCost = gCost;
            this.hCost = hCost;
        }
        public Vector3 Position { get; set; }
        public bool Walkable { get; set; }

        public float gCost { get; set; }
        public float hCost { get; set; }
        public float fCost => this.gCost + this.hCost;

        public override string ToString() => $"Node (Pos={this.Position} Walkable={this.Walkable} gCost={this.gCost} hCost={this.hCost} fCost={this.fCost})";
    }
}

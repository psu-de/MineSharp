using MineSharp.Core.Types;
using MineSharp.Pathfinding.Moves;
using Priority_Queue;

namespace MineSharp.Pathfinding.Algorithm
{
    public class Node : FastPriorityQueueNode
    {
        public Vector3 Position { get; set; }
        public bool Walkable { get; set; }

        public float gCost { get; set; }
        public float hCost { get; set; }
        public float fCost => gCost + hCost;


        public Node? Parent;

        public Node(Vector3 position, bool walkable, int gCost, int hCost)
        {
            Position = position;
            Walkable = walkable;
            this.gCost = gCost;
            this.hCost = hCost;
        }

        public override string ToString()
        {
            return $"Node (Pos={Position} Walkable={Walkable} gCost={gCost} hCost={hCost} fCost={fCost})";
        }
    }
}

using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
using MineSharp.Pathfinder.Moves;

namespace MineSharp.Pathfinder;

public class Path
{
    /// <summary>
    /// The array of moves for this path
    /// </summary>
    public PathNode[] Nodes;

    private Path(PathNode[] nodes)
    {
        this.Nodes = nodes;
    }

    /// <summary>
    /// Construct a <see cref="Path"/> from an array of moves.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="moves"></param>
    /// <returns></returns>
    public static Path FromMoves(Vector3 start, Move[] moves)
    {
        var nodes = new List<PathNode>();
        var currentPoint = start.Clone();

        nodes.Add(new PathNode(start.Clone(), moves[0], 1));

        var previous = moves[0];
        for (int i = 1; i < moves.Length; i++)
        {
            var current = moves[i];

            if (current.CanBeLinked 
                && current.GetType() == previous.GetType() 
                && current.Motion == previous.Motion)
            {
                nodes[^1].Count++;
            }
            else
            {
                var last  = nodes[^1];
                var delta = last.Move.Motion.Scaled(last.Count);
                var point = currentPoint.Add(delta).Clone();
                
                nodes.Add(new PathNode(point, current, 1));
            }

            previous = current;
        }

        return new Path(nodes.ToArray());
    }

    /// <summary>
    /// A path node.
    /// </summary>
    /// <param name="Position">Where this node starts</param>
    /// <param name="Move">The move to perform</param>
    /// <param name="Count">How often the move should be performed</param>
    public class PathNode(Vector3 position, Move move, int count)
    {
        public readonly Vector3 Position = position;
        public readonly Move    Move     = move;

        public int Count { get; set; } = count;
    }
}

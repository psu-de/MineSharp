using MineSharp.Core.Types;

namespace MineSharp.Pathfinding.Goals
{
    public abstract class Goal
    {
        public abstract Vector3 Target { get; }
    }
}

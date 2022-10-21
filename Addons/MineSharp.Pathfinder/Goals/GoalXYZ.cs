using MineSharp.Core.Types;

namespace MineSharp.Pathfinding.Goals
{
    public class GoalXYZ : Goal
    {

        public GoalXYZ(int x, int y, int z)
        {
            this.Target = new Vector3(x, y, z);
        }
        public override Vector3 Target { get; }
    }
}

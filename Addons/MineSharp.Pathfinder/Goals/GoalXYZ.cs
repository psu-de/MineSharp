using MineSharp.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Pathfinding.Goals
{
    public class GoalXYZ : Goal
    {
        public override Vector3 Target { get; }

        public GoalXYZ(int x, int y, int z) : base()
        {
            this.Target = new Vector3(x, y, z);
        }



    }
}

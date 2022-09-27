using MineSharp.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Pathfinding.Goals
{
    public abstract class Goal
    {
        public abstract Vector3 Target { get; }

    }
}

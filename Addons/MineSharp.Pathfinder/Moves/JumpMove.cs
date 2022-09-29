using MineSharp.Core.Types;
using MineSharp.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MineSharp.Bot;

namespace MineSharp.Pathfinding.Moves
{
    public class JumpMove : Move
    {
        private Vector3 _direction;
        public override Vector3 MoveVector => _direction;

        internal JumpMove(Movements movements, Vector3 direction) : base(movements)
        {
            this._direction = direction.Plus(Vector3.Up);
        }

        public override Task PerformMove(MinecraftBot bot)
        {
            throw new NotImplementedException();
        }
    }
}

using MineSharp.Bot;
using MineSharp.Core.Types;

namespace MineSharp.Pathfinding.Moves
{
    public class DownMove : Move
    {
        private Vector3 _direction;
        public override Vector3 MoveVector => _direction;

        internal DownMove(Movements movements, Vector3 direction) : base(movements)
        {
            this._direction = direction.Plus(Vector3.Down);
        }


        public override Task PerformMove(MinecraftBot bot)
        {
            throw new NotImplementedException();
        }
    }
}

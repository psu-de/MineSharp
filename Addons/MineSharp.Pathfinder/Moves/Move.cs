using MineSharp.Bot;
using MineSharp.Core.Types;

namespace MineSharp.Pathfinding.Moves
{
    public abstract class Move
    {
        protected Movements Movements { get; private set; }

        /// <summary>
        /// The relative position after the mvoe
        /// </summary>
        public abstract Vector3 MoveVector { get; }

        internal Move(Movements movements)
        {
            Movements = movements;
        }

        /// <summary>
        /// Perform the move
        /// </summary>
        /// <param name="bot">The <see cref="MinecraftBot"/> that should perform the move</param>
        /// <returns></returns>
        public abstract Task PerformMove(MinecraftBot bot);
    }
}

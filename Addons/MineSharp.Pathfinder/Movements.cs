using MineSharp.Core.Types;
using MineSharp.Pathfinding.Moves;

namespace MineSharp.Pathfinding
{
    public class Movements
    {
        public bool AllowSprinting { get; set; }
        public bool AllowJumping { get; set; }

        public Move[] PossibleMoves { get; private set; }

        public Movements(bool allowSprinting, bool allowJumping)
        {
            this.AllowSprinting = allowSprinting;
            this.AllowJumping = allowJumping;

            this.PossibleMoves = this.GetPossibleMoves();
        }

        public Movements() : this(
            true,
            true) {}

        public Move GetMoveByVector(Vector3 movement)
        {
            var vec = this.PossibleMoves.FirstOrDefault(x => x.MoveVector == movement);
            if (vec == null)
            {
                throw new Exception($"Move not found");
            }

            return vec;
        }

        private Move[] GetPossibleMoves()
        {
            var moves = new List<Move>();

            var directions = new Vector3[] {
                // direct neighbors
                Vector3.North,
                Vector3.East,
                Vector3.South,
                Vector3.West
            };

            moves.AddRange(directions.Select(dir => new DirectMove(this, dir)));
            moves.AddRange(directions.Select(dir => new DownMove(this, dir)));

            if (this.AllowJumping)
            {
                moves.AddRange(directions.Select(dir => new JumpUpMove(this, dir)));
            }

            return moves.ToArray();
        }
    }
}

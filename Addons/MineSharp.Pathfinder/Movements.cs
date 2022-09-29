using MineSharp.Core.Types;
using MineSharp.Pathfinding.Moves;

namespace MineSharp.Pathfinding
{
    public class Movements
    {
        public bool AllowSprinting { get; set; }
        public bool AllowJumping { get; set; }

        public Move[] PossibleMoves { get; private set;  }
        
        public Movements(bool allowSprinting, bool allowJumping)
        {
            AllowSprinting = allowSprinting;
            AllowJumping = allowJumping;

            this.PossibleMoves = GetPossibleMoves();
        }

        public Movements() : this( 
                allowSprinting: true,
                allowJumping: true)
        { }

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
            List<Move> moves = new();

            Vector3[] directions = new Vector3[] {
                // direct neighbors
                Vector3.North,
                Vector3.East,
                Vector3.South,
                Vector3.West,
                //diagonal neighbors
                // Vector3.North.Plus(Vector3.East),
                // Vector3.North.Plus(Vector3.West),
                // Vector3.South.Plus(Vector3.East),
                // Vector3.South.Plus(Vector3.West),
            };

            foreach (var direction in directions)
            {
                moves.Add(new DirectMove(this, direction));
            }

            // foreach (var direction in directions)
            // {
            //     moves.Add(new DownMove(this, direction));
            // }
            //
            // if (this.AllowJumping)
            // {
            //     foreach (var direction in directions)
            //     {
            //         moves.Add(new JumpMove(this, direction));
            //     }
            // }

            return moves.ToArray();
        }
    }
}

using MineSharp.Core.Common;
using MineSharp.Pathfinder.Moves;

namespace MineSharp.Pathfinder;


public class Movements
{
    private static readonly Vector3[] Directions =
    [
        Vector3.North,
        Vector3.East,
        Vector3.South,
        Vector3.West
    ];

    private static readonly Vector3[] DiagonalDirections =
    [
        Vector3.North.Plus(Vector3.East),
        Vector3.North.Plus(Vector3.West),
        Vector3.South.Plus(Vector3.East),
        Vector3.South.Plus(Vector3.West)
    ];

    public static readonly Movements Default = new(true, true);
    
    public bool AllowSprinting { get; }

    public bool AllowJumping { get; }

    public readonly Move[] PossibleMoves;
    
    public Movements(bool allowSprinting, bool allowJumping)
    {
        this.AllowSprinting = allowSprinting;
        this.AllowJumping = allowJumping;
        
        var moves = new List<Move>();
        
        moves.AddRange(Directions.Select(x => new DirectMove(x)));
        moves.AddRange(DiagonalDirections.Select(x => new DirectMove(x)));

        if (this.AllowJumping)
        {
            moves.AddRange(Directions.Select(x => new JumpUpMove(x)));
            moves.AddRange(DiagonalDirections.Select(x => new JumpUpMove(x)));
            
            moves.AddRange(Directions.Select(x => new JumpOneBlock(x)));
            moves.AddRange(DiagonalDirections.Select(x => new JumpOneBlock(x)));
        }
        
        moves.AddRange(Directions.Select(x => new FallDownMove(x)));
        moves.AddRange(DiagonalDirections.Select(x => new FallDownMove(x)));

        this.PossibleMoves = moves.OrderBy(x => x.Cost).ToArray();
    }
}

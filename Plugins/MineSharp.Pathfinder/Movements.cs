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
    
    public static readonly Movements Default = new ()
    {
        AllowJumping = true,
        AllowSprinting = true
    };
    
    public required bool AllowSprinting { get; init; }

    public required bool AllowJumping { get; init; }

    public readonly IMove[] PossibleMoves;
    
    public Movements()
    {
        var moves = new List<IMove>();
        
        moves.AddRange(Directions.Select(x => new DirectMove(x)));
        moves.AddRange(DiagonalDirections.Select(x => new DirectMove(x)));

        this.PossibleMoves = moves.ToArray();
    }
}
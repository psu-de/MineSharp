using MineSharp.Core.Exceptions;

namespace MineSharp.Pathfinder.Exceptions;

/// <summary>
/// Exception thrown when a <see cref="Moves.Move"/> could not be done.
/// </summary>
/// <param name="message"></param>
public class MoveWentWrongException(string message) : MineSharpException(message)
{ }

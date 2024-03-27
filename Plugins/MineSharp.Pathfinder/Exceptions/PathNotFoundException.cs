using MineSharp.Core.Exceptions;

namespace MineSharp.Pathfinder.Exceptions;

/// <summary>
/// Exception thrown when not path could be found.
/// </summary>
/// <param name="message"></param>
public class PathNotFoundException(string message) : MineSharpException(message)
{ }
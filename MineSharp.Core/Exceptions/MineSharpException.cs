namespace MineSharp.Core.Exceptions;

/// <summary>
///     Exception thrown by MineSharp projects
/// </summary>
/// <param name="message"></param>
public abstract class MineSharpException(string message) : Exception(message);

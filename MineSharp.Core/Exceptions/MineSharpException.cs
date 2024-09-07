namespace MineSharp.Core.Exceptions;

/// <summary>
///     Exception thrown by MineSharp projects.
/// </summary>
/// <param name="message"></param>
/// <param name="innerException"></param>
public abstract class MineSharpException(string message, Exception? innerException = null) : Exception(message, innerException);

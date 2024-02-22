using MineSharp.Core.Exceptions;

namespace MineSharp.Data.Exceptions;

/// <summary>
/// Exception thrown when a version is not supported by MineSharp.Data.
/// </summary>
/// <param name="message"></param>
public class MineSharpVersionNotSupportedException(string message) : MineSharpException(message);
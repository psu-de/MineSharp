using MineSharp.Core.Exceptions;

namespace MineSharp.Auth.Exceptions;

/// <summary>
/// Thrown when MineSharp could not authenticate.
/// </summary>
/// <param name="message"></param>
public class MineSharpAuthException(string message) : MineSharpException(message);

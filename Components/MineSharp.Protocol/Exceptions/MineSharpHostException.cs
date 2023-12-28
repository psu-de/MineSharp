using MineSharp.Core.Exceptions;

namespace MineSharp.Protocol.Exceptions;

/// <inheritdoc />
public class MineSharpHostException(string message) : MineSharpException(message);

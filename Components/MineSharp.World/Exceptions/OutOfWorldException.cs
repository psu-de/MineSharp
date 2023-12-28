using MineSharp.Core.Exceptions;

namespace MineSharp.World.Exceptions;

/// <inheritdoc />
public class OutOfWorldException(string message) : MineSharpException(message);

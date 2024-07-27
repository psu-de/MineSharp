using MineSharp.Core.Exceptions;

namespace MineSharp.World.Exceptions;

/// <inheritdoc />
public class ChunkNotLoadedException(string message) : MineSharpException(message);

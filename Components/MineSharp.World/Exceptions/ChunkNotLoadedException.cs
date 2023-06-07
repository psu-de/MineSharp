using MineSharp.Core.Exceptions;

namespace MineSharp.World.Exceptions;

public class ChunkNotLoadedException : MineSharpException
{

    public ChunkNotLoadedException(string message) : base(message)
    { }
}

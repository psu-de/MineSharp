using MineSharp.Core.Exceptions;

namespace MineSharp.World.Exceptions;

public class OutOfWorldException : MineSharpException
{
    public OutOfWorldException(string message) : base(message)
    { }
}

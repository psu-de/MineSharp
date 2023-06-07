using MineSharp.Core.Exceptions;

namespace MineSharp.Protocol.Exceptions;

public class UnexpectedPacketException : MineSharpException
{

    public UnexpectedPacketException(string message) : base(message)
    { }
}

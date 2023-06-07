using MineSharp.Core.Exceptions;

namespace MineSharp.Protocol.Exceptions;

public class MineSharpHostException : MineSharpException
{

    public MineSharpHostException(string message) : base(message) {}
}

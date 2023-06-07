using MineSharp.Core.Exceptions;

namespace MineSharp.Auth.Exceptions;

public class MineSharpAuthException : MineSharpException
{
    public MineSharpAuthException(string message) : base(message) {}
}

using MineSharp.Core.Exceptions;

namespace MineSharp.Data.Exceptions;

public class MineSharpDataException : MineSharpException
{
    public MineSharpDataException(string message) : base(message) 
    { }
}

using MineSharp.Core.Exceptions;

namespace MineSharp.Data.Exceptions;

public class MineSharpVersionNotFoundException : MineSharpException
{
    public MineSharpVersionNotFoundException(string message) : base(message) 
    { }
}

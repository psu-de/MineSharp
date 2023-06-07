namespace MineSharp.Core.Exceptions;

public abstract class MineSharpException : Exception
{
    public MineSharpException(string message) : base(message) 
    { }
}

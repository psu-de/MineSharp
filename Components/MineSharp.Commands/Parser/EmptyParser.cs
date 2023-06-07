namespace MineSharp.Commands.Parser;

public class EmptyParser : IParser
{
    public string GetName() => string.Empty;

    public int GetArgumentCount() => 1;
}

namespace MineSharp.Commands.Parser;

public class IntegerParser : IParser
{
    public readonly int Min;
    public readonly int Max;

    public IntegerParser(int min = int.MinValue, int max = int.MaxValue)
    {
        this.Min = min;
        this.Max = max;
    }

    public string GetName()          => "brigadier:integer";
    public int    GetArgumentCount() => 1;
}

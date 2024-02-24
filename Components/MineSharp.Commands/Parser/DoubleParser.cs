namespace MineSharp.Commands.Parser;

public class DoubleParser : IParser
{
    public readonly double Min;
    public readonly double Max;

    public DoubleParser(double min = double.MinValue, double max = double.MaxValue)
    {
        this.Min = min;
        this.Max = max;
    }

    public string GetName()          => "brigadier:double";
    public int    GetArgumentCount() => 1;
}

namespace MineSharp.Commands.Parser;

public class FloatParser : IParser
{
    public readonly float Min;
    public readonly float Max;

    public FloatParser(float min = float.MinValue, float max = float.MaxValue)
    {
        this.Min = min;
        this.Max = max;
    }

    public string GetName()          => "brigadier:float";
    public int    GetArgumentCount() => 1;
}

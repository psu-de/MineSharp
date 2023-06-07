namespace MineSharp.Commands.Parser;

public class LongParser : IParser
{
    public readonly long Min;
    public readonly long Max;
    
    public LongParser(long min = long.MinValue, long max = long.MaxValue)
    {
        this.Min = min;
        this.Max = max;
    }
    
    public string GetName() => "brigadier:long";
    public int GetArgumentCount() => 1;
}

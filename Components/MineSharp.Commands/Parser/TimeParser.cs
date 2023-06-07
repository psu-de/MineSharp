namespace MineSharp.Commands.Parser;

public class TimeParser : IParser
{

    public TimeParser(int t)
    {
        
    }

    public string GetName() => "minecraft:time";
    public int GetArgumentCount() => 1;
}

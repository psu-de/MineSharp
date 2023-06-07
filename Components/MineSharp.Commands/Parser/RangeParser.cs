namespace MineSharp.Commands.Parser;

public class RangeParser : IParser
{
    public readonly bool Decimals;

    public RangeParser(bool decimals)
    {
        this.Decimals = decimals;
    }


    public string GetName() => "minecraft:range";
    public int GetArgumentCount() => 1;
}

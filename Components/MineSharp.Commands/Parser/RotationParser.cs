namespace MineSharp.Commands.Parser;

public class RotationParser : IParser
{
    public string GetName() => "minecraft:rotation";
    public int GetArgumentCount() => 2;
}

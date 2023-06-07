namespace MineSharp.Commands.Parser;

public class MessageParser : IParser
{
    public string GetName() => "minecraft:message";
    public int GetArgumentCount() => 1;
}

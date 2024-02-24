namespace MineSharp.Commands.Parser;

public class BlockPositionParser : IParser
{
    public string GetName()          => "minecraft:block_pos";
    public int    GetArgumentCount() => 3;
}

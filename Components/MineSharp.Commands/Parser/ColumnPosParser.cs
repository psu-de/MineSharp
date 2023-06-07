namespace MineSharp.Commands.Parser;

public class ColumnPosParser : IParser
{
    public string GetName() => "minecraft:column_pos";
    public int GetArgumentCount() => 3;
}

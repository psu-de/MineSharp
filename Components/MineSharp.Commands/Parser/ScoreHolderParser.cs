namespace MineSharp.Commands.Parser;

public class ScoreHolderParser : IParser
{
    public readonly byte Flags;
        
    public ScoreHolderParser(byte flags)
    {
        this.Flags = flags;
    }

    public string GetName() => "minecraft:score_holder";
    public int GetArgumentCount() => 1;
}

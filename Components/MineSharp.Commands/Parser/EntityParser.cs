namespace MineSharp.Commands.Parser;

public class EntityParser : IParser
{
    public readonly byte Flags;

    public EntityParser(byte flags)
    {
        this.Flags = flags;
    }

    public string GetName()          => "minecraft:entity";
    public int    GetArgumentCount() => 1;
}

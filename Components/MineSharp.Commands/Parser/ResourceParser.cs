namespace MineSharp.Commands.Parser;

public class ResourceParser : IParser
{
    public readonly string Registry;

    public ResourceParser(string registry)
    {
        this.Registry = registry;
    }


    public string GetName()          => "minecraft:resource";
    public int    GetArgumentCount() => 1;
}

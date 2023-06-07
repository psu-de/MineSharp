namespace MineSharp.Commands.Parser;

public class ResourceOrTagParser : IParser
{
    private readonly string Registry;

    public ResourceOrTagParser(string registry)
    {
        this.Registry = registry;
    }
    
    public string GetName() => "minecraft:resource_or_tag";
    public int GetArgumentCount() => 1;
}

namespace MineSharp.Commands.Parser;

public class Vec3Parser : IParser
{
    public string GetName() => "minecraft:vec3";
    public int GetArgumentCount() => 3;
}

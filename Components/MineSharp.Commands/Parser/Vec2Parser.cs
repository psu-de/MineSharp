using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class Vec2Parser : IParser
{
    public string GetName()
    {
        return "minecraft:vec2";
    }

    public int GetArgumentCount()
    {
        return 2;
    }

    public void ReadProperties(PacketBuffer buffer, MinecraftData data)
    { }
}

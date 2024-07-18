using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class Vec3Parser : IParser
{
    public string GetName()
    {
        return "minecraft:vec3";
    }

    public int GetArgumentCount()
    {
        return 3;
    }

    public void ReadProperties(PacketBuffer buffer, MinecraftData data)
    { }
}

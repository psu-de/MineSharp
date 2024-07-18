using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class RotationParser : IParser
{
    public string GetName()
    {
        return "minecraft:rotation";
    }

    public int GetArgumentCount()
    {
        return 2;
    }

    public void ReadProperties(PacketBuffer buffer, MinecraftData data)
    { }
}

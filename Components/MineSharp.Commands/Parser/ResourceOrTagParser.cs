using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class ResourceOrTagParser : IParser
{
    public string Registry { get; private set; } = string.Empty;

    public string GetName()
    {
        return "minecraft:resource_or_tag";
    }

    public int GetArgumentCount()
    {
        return 1;
    }

    public void ReadProperties(PacketBuffer buffer, MinecraftData data)
    {
        Registry = buffer.ReadString();
    }
}

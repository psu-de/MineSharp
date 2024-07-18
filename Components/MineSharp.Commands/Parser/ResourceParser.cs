using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class ResourceParser : IParser
{
    public string? Registry { get; private set; }


    public string GetName()
    {
        return "minecraft:resource";
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

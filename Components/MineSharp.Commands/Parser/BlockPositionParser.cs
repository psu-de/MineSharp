using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class BlockPositionParser : IParser
{
    public string GetName()
    {
        return "minecraft:block_pos";
    }

    public int GetArgumentCount()
    {
        return 3;
    }

    public void ReadProperties(PacketBuffer buffer, MinecraftData data)
    { }
}

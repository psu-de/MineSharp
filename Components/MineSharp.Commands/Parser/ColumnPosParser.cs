using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class ColumnPosParser : IParser
{
    public string GetName()
    {
        return "minecraft:column_pos";
    }

    public int GetArgumentCount()
    {
        return 3;
    }

    public void ReadProperties(PacketBuffer buffer, MinecraftData data)
    { }
}

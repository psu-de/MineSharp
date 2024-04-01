using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class ColumnPosParser : IParser
{
    public string GetName()          => "minecraft:column_pos";
    public int    GetArgumentCount() => 3;
    public void   ReadProperties(PacketBuffer buffer, MinecraftData data)
    { }
}

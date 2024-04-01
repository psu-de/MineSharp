using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class BlockPositionParser : IParser
{
    public string GetName()          => "minecraft:block_pos";
    public int    GetArgumentCount() => 3;
    
    public void   ReadProperties(PacketBuffer buffer, MinecraftData data)
    { }
}

using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class Vec2Parser : IParser
{
    public string GetName()          => "minecraft:vec2";
    public int    GetArgumentCount() => 2;
    
    public void   ReadProperties(PacketBuffer buffer, MinecraftData data)
    { }
}

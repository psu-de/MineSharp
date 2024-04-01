using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class RotationParser : IParser
{
    public string GetName()          => "minecraft:rotation";
    public int    GetArgumentCount() => 2;
    
    public void   ReadProperties(PacketBuffer buffer, MinecraftData data)
    { }
}

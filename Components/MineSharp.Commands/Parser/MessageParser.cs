using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class MessageParser : IParser
{
    public string GetName()          => "minecraft:message";
    public int    GetArgumentCount() => 1;
    public void   ReadProperties(PacketBuffer buffer, MinecraftData data)
    { }
}

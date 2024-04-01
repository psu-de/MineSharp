using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class ResourceParser : IParser
{
    public string Registry { get; private set; }


    public string GetName()          => "minecraft:resource";
    public int    GetArgumentCount() => 1;
    
    public void   ReadProperties(PacketBuffer buffer, MinecraftData data)
    {
        this.Registry = buffer.ReadString();
    }
}

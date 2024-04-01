using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class ResourceOrTagParser : IParser
{
    public string Registry { get; private set; } = string.Empty;

    public string GetName()          => "minecraft:resource_or_tag";
    public int    GetArgumentCount() => 1;
    public void   ReadProperties(PacketBuffer buffer, MinecraftData data)
    {
        this.Registry = buffer.ReadString();
    }
}

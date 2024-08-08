using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class ResourceOrTagParser : IParser
{
    public static readonly Identifier ResourceOrTagIdentifier = Identifier.Parse("minecraft:resource_or_tag");

    public string Registry { get; private set; } = string.Empty;

    public Identifier GetName()
    {
        return ResourceOrTagIdentifier;
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

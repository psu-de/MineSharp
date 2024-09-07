using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class ResourceParser : IParser
{
    public static readonly Identifier ResourceIdentifier = Identifier.Parse("minecraft:resource");

    public string? Registry { get; private set; }

    public Identifier GetName()
    {
        return ResourceIdentifier;
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

using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class RotationParser : IParser
{
    public static readonly Identifier RotationIdentifier = Identifier.Parse("minecraft:rotation");

    public Identifier GetName()
    {
        return RotationIdentifier;
    }

    public int GetArgumentCount()
    {
        return 2;
    }

    public void ReadProperties(PacketBuffer buffer, MinecraftData data)
    { }
}

using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class MessageParser : IParser
{
    public static readonly Identifier MessageIdentifier = Identifier.Parse("minecraft:message");

    public Identifier GetName()
    {
        return MessageIdentifier;
    }

    public int GetArgumentCount()
    {
        return 1;
    }

    public void ReadProperties(PacketBuffer buffer, MinecraftData data)
    { }
}

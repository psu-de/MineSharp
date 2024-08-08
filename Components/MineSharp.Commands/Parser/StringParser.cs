using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class StringParser : IParser
{
    public static readonly Identifier BrigadierStringIdentifier = Identifier.Parse("brigadier:string");

    public StringType Type { get; private set; }

    public Identifier GetName()
    {
        return BrigadierStringIdentifier;
    }

    public int GetArgumentCount()
    {
        return 1;
    }

    public void ReadProperties(PacketBuffer buffer, MinecraftData data)
    {
        Type = (StringType)buffer.ReadVarInt();
    }
}

public enum StringType
{
    SingleWord = 0,
    QuotablePhrase = 1,
    GreedyPhrase = 2
}

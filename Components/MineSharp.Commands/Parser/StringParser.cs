using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class StringParser : IParser
{
    public StringType Type { get; private set; }

    public string GetName()
    {
        return "brigadier:string";
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

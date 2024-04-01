using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class StringParser : IParser
{
    public StringType Type { get; private set; }

    public string GetName()          => "brigadier:string";
    public int    GetArgumentCount() => 1;
    
    public void   ReadProperties(PacketBuffer buffer, MinecraftData data)
    {
        this.Type = (StringType)buffer.ReadVarInt();
    }
}

public enum StringType
{
    SINGLE_WORD = 0,
    QUOTABLE_PHRASE = 1,
    GREEDY_PHRASE = 2
}

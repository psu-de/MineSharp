using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class ColumnPosParser : IParser
{
    public static readonly Identifier ColumnPosIdentifier = Identifier.Parse("minecraft:column_pos");

    public Identifier GetName()
    {
        return ColumnPosIdentifier;
    }

    public int GetArgumentCount()
    {
        return 3;
    }

    public void ReadProperties(PacketBuffer buffer, MinecraftData data)
    { }
}

using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class IntegerParser : IParser
{
    public static readonly Identifier BrigadierIntegerIdentifier = Identifier.Parse("brigadier:integer");

    public int Min { get; private set; }
    public int Max { get; private set; }

    public Identifier GetName()
    {
        return BrigadierIntegerIdentifier;
    }

    public int GetArgumentCount()
    {
        return 1;
    }

    public void ReadProperties(PacketBuffer buffer, MinecraftData data)
    {
        var flags = buffer.ReadByte();
        Min = (flags & 0x01) > 0 ? buffer.ReadInt() : int.MinValue;
        Max = (flags & 0x02) > 0 ? buffer.ReadInt() : int.MaxValue;
    }
}

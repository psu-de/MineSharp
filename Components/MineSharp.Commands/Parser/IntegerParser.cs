using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class IntegerParser : IParser
{
    public int Min { get; private set; }
    public int Max { get; private set; }

    public string GetName()
    {
        return "brigadier:integer";
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

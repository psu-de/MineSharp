using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class DoubleParser : IParser
{
    public double Min { get; private set; }
    public double Max { get; private set; }

    public string GetName()
    {
        return "brigadier:double";
    }

    public int GetArgumentCount()
    {
        return 1;
    }

    public void ReadProperties(PacketBuffer buffer, MinecraftData data)
    {
        var flags = buffer.ReadByte();
        Min = (flags & 0x01) > 0 ? buffer.ReadDouble() : double.MinValue;
        Max = (flags & 0x02) > 0 ? buffer.ReadDouble() : double.MaxValue;
    }
}

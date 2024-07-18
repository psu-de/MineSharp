using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class FloatParser : IParser
{
    public float Min { get; private set; }
    public float Max { get; private set; }

    public string GetName()
    {
        return "brigadier:float";
    }

    public int GetArgumentCount()
    {
        return 1;
    }

    public void ReadProperties(PacketBuffer buffer, MinecraftData data)
    {
        var flags = buffer.ReadByte();
        Min = (flags & 0x01) > 0 ? buffer.ReadFloat() : float.MinValue;
        Max = (flags & 0x02) > 0 ? buffer.ReadFloat() : float.MaxValue;
    }
}

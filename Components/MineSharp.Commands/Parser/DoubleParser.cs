using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class DoubleParser : IParser
{
    public static readonly Identifier BrigadierDoubleIdentifier = Identifier.Parse("brigadier:double");

    public double Min { get; private set; }
    public double Max { get; private set; }

    public Identifier GetName()
    {
        return BrigadierDoubleIdentifier;
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

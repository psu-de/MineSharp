using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class FloatParser : IParser
{
    public static readonly Identifier BrigadierFloatIdentifier = Identifier.Parse("brigadier:float");

    public float Min { get; private set; }
    public float Max { get; private set; }

    public Identifier GetName()
    {
        return BrigadierFloatIdentifier;
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

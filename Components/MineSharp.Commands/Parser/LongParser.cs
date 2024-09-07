using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class LongParser : IParser
{
    public static readonly Identifier BrigadierLongIdentifier = Identifier.Parse("brigadier:long");

    public long Min { get; private set; }
    public long Max { get; private set; }

    public Identifier GetName()
    {
        return BrigadierLongIdentifier;
    }

    public int GetArgumentCount()
    {
        return 1;
    }

    public void ReadProperties(PacketBuffer buffer, MinecraftData data)
    {
        var flags = buffer.ReadByte();
        Min = (flags & 0x01) > 0 ? buffer.ReadLong() : long.MinValue;
        Max = (flags & 0x02) > 0 ? buffer.ReadLong() : long.MaxValue;
    }
}

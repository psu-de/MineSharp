using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class LongParser : IParser
{
    public long Min { get; private set; }
    public long Max { get; private set; }

    public string GetName()          => "brigadier:long";
    public int    GetArgumentCount() => 1;
    public void   ReadProperties(PacketBuffer buffer, MinecraftData data)
    {
        var flags = buffer.ReadByte();
        this.Min = (flags & 0x01) > 0 ? buffer.ReadLong() : long.MinValue;
        this.Max = (flags & 0x02) > 0 ? buffer.ReadLong() : long.MaxValue;
    }
}

using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class FloatParser : IParser
{
    public float Min { get; private set; }
    public float Max { get; private set; }

    public string GetName()          => "brigadier:float";
    public int    GetArgumentCount() => 1;
    
    public void   ReadProperties(PacketBuffer buffer, MinecraftData data)
    {
        var flags = buffer.ReadByte();
        this.Min = (flags & 0x01) > 0 ? buffer.ReadFloat() : float.MinValue;
        this.Max = (flags & 0x02) > 0 ? buffer.ReadFloat() : float.MaxValue;
    }
}

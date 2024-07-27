using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class RangeParser : IParser
{
    public bool Decimals { get; private set; }

    public string GetName()
    {
        return "minecraft:range";
    }

    public int GetArgumentCount()
    {
        return 1;
    }

    public void ReadProperties(PacketBuffer buffer, MinecraftData data)
    {
        Decimals = buffer.ReadBool();
    }
}

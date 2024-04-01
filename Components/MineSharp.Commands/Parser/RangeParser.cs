using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class RangeParser : IParser
{
    public bool Decimals { get; private set; }
    
    public string GetName() => "minecraft:range";
    public int GetArgumentCount() => 1;

    public void ReadProperties(PacketBuffer buffer, MinecraftData data)
    {
        this.Decimals = buffer.ReadBool();
    }
}

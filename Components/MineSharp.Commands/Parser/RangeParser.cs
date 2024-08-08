using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class RangeParser : IParser
{
    public static readonly Identifier RangeIdentifier = Identifier.Parse("minecraft:range");

    public bool Decimals { get; private set; }

    public Identifier GetName()
    {
        return RangeIdentifier;
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

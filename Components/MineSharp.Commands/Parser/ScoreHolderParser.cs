using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class ScoreHolderParser : IParser
{
    public static readonly Identifier ScoreHolderIdentifier = Identifier.Parse("minecraft:score_holder");

    public byte Flags { get; private set; }

    public Identifier GetName()
    {
        return ScoreHolderIdentifier;
    }

    public int GetArgumentCount()
    {
        return 1;
    }

    public void ReadProperties(PacketBuffer buffer, MinecraftData data)
    {
        Flags = buffer.ReadByte();
    }
}

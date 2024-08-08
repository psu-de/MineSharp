using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class BlockPositionParser : IParser
{
    public static readonly Identifier BlockPosIdentifier = Identifier.Parse("minecraft:block_pos");

    public Identifier GetName()
    {
        return BlockPosIdentifier;
    }

    public int GetArgumentCount()
    {
        return 3;
    }

    public void ReadProperties(PacketBuffer buffer, MinecraftData data)
    { }
}

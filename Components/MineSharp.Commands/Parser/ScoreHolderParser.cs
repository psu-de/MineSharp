using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class ScoreHolderParser : IParser
{
    public byte Flags { get; private set; }

    public string GetName()
    {
        return "minecraft:score_holder";
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

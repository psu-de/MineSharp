using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class EntityParser : IParser
{
    public byte Flags { get; private set; }

    public string GetName()
    {
        return "minecraft:entity";
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

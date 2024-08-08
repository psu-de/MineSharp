using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class EntityParser : IParser
{
    public static readonly Identifier EntityIdentifier = Identifier.Parse("minecraft:entity");

    public byte Flags { get; private set; }

    public Identifier GetName()
    {
        return EntityIdentifier;
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

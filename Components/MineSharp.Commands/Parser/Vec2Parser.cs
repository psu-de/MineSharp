using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class Vec2Parser : IParser
{
    public static readonly Identifier Vec2Identifier = Identifier.Parse("minecraft:vec2");

    public Identifier GetName()
    {
        return Vec2Identifier;
    }

    public int GetArgumentCount()
    {
        return 2;
    }

    public void ReadProperties(PacketBuffer buffer, MinecraftData data)
    { }
}

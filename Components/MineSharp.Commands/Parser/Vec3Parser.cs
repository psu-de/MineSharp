using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class Vec3Parser : IParser
{
    public static readonly Identifier Vec3Identifier = Identifier.Parse("minecraft:vec3");

    public Identifier GetName()
    {
        return Vec3Identifier;
    }

    public int GetArgumentCount()
    {
        return 3;
    }

    public void ReadProperties(PacketBuffer buffer, MinecraftData data)
    { }
}

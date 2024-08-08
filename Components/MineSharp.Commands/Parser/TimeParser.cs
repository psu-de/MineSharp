using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class TimeParser : IParser
{
    public static readonly Identifier TimeIdentifier = Identifier.Parse("minecraft:time");

    public int? Min { get; private set; }

    public Identifier GetName()
    {
        return TimeIdentifier;
    }

    public int GetArgumentCount()
    {
        return 1;
    }

    public void ReadProperties(PacketBuffer buffer, MinecraftData data)
    {
        if (data.Version.Protocol <= ProtocolVersion.V_1_19_3)
        {
            return;
        }

        Min = buffer.ReadInt();
    }
}

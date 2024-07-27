using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class TimeParser : IParser
{
    public int? Min { get; private set; }

    public string GetName()
    {
        return "minecraft:time";
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

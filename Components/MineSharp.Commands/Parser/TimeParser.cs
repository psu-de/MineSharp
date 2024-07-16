using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class TimeParser : IParser
{
    public int?   Min                { get; private set; } = null;
    
    public string GetName()          => "minecraft:time";
    public int    GetArgumentCount() => 1;
    
    public void   ReadProperties(PacketBuffer buffer, MinecraftData data)
    {
        if (data.Version.Protocol <= ProtocolVersion.V_1_19_3)
        {
            return;
        }

        this.Min = buffer.ReadInt();
    }
}

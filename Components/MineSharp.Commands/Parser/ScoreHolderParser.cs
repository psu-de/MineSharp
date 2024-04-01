using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class ScoreHolderParser : IParser
{
    public byte Flags { get; private set; }

    public string GetName()          => "minecraft:score_holder";
    public int    GetArgumentCount() => 1;
    
    public void   ReadProperties(PacketBuffer buffer, MinecraftData data)
    {
        this.Flags = buffer.ReadByte();
    }
}

using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class EntityParser : IParser
{
    public byte Flags { get; private set; }

    public string GetName()          => "minecraft:entity";
    public int    GetArgumentCount() => 1;
    public void   ReadProperties(PacketBuffer buffer, MinecraftData data)
    {
        this.Flags = buffer.ReadByte();
    }
}

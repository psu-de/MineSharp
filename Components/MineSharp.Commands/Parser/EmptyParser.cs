using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public class EmptyParser : IParser
{
    public string GetName() => string.Empty;

    public int  GetArgumentCount() => 1;
    
    public void ReadProperties(PacketBuffer buffer, MinecraftData data)
    { }
}

using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Commands.Parser;

public interface IParser
{
    public string GetName();
    public int GetArgumentCount();

    public void ReadProperties(PacketBuffer buffer, MinecraftData data);
}

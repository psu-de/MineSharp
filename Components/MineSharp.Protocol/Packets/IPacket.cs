using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets;

public interface IPacket
{
    public PacketType Type { get; }
    
    public void Write(PacketBuffer buffer, MinecraftData version);

    public abstract static IPacket Read(PacketBuffer buffer, MinecraftData version);
}

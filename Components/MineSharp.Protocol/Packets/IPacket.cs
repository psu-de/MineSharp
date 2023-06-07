using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets;

public interface IPacket
{
    public abstract static int Id { get; }
    
    public void Write(PacketBuffer buffer, MinecraftData version, string packetName);

    public abstract static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName);
}

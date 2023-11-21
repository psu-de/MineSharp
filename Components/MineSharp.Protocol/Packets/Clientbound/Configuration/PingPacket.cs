using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;

public class PingPacket : IPacket
{
    public PacketType Type => PacketType.CB_Configuration_Ping;
 
    public int Id { get; set; }

    public PingPacket(int id)
    {
        this.Id = id;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteInt(this.Id);
    }
    
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new PingPacket(
            buffer.ReadInt());
    }
}

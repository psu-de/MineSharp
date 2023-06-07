using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Serverbound.Status;

public class PingRequestPacket : IPacket
{
    public static int Id => 0x01;

    public long Payload { get; set; }

    public PingRequestPacket(long payload)
    {
        this.Payload = payload;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteLong(this.Payload);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        return new PingRequestPacket(buffer.ReadLong());
    }
}

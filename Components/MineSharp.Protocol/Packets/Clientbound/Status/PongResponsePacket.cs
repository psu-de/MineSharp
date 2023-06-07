using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Clientbound.Status;

public class PongResponsePacket : IPacket
{
    public static int Id => 0x01;

    public long Payload { get; set; }

    public PongResponsePacket(long payload)
    {
        this.Payload = payload;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteLong(this.Payload);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        return new PongResponsePacket(buffer.ReadLong());
    }
}

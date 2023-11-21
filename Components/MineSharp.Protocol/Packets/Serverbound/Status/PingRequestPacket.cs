using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Status;

public class PingRequestPacket : IPacket
{
    public PacketType Type => PacketType.SB_Status_Ping;

    public long Payload { get; set; }

    public PingRequestPacket(long payload)
    {
        this.Payload = payload;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteLong(this.Payload);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new PingRequestPacket(buffer.ReadLong());
    }
}

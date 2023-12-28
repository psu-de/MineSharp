using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Status;
#pragma warning disable CS1591
public class PongResponsePacket : IPacket
{
    public PacketType Type => PacketType.CB_Status_Ping;

    public long Payload { get; set; }

    public PongResponsePacket(long payload)
    {
        this.Payload = payload;
    }
    
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteLong(this.Payload);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new PongResponsePacket(buffer.ReadLong());
    }
}
#pragma warning restore CS1591
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Status;
#pragma warning disable CS1591
public class PingRequestPacket : IPacket
{
    public PingRequestPacket(long payload)
    {
        Payload = payload;
    }

    public long Payload { get; set; }
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.SB_Status_Ping;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteLong(Payload);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new PingRequestPacket(buffer.ReadLong());
    }
}
#pragma warning restore CS1591

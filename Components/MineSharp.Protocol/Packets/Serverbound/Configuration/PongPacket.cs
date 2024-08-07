using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Configuration;
#pragma warning disable CS1591
public class PongPacket : IPacket
{
    public PongPacket(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.SB_Configuration_Pong;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteInt(Id);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new PongPacket(
            buffer.ReadInt());
    }
}
#pragma warning restore CS1591

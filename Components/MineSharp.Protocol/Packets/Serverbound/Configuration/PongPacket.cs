using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Configuration;
#pragma warning disable CS1591
public class PongPacket : IPacket
{
    public PacketType Type => PacketType.SB_Configuration_Pong;

    public int Id { get; set; }

    public PongPacket(int id)
    {
        this.Id = id;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteInt(this.Id);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new PongPacket(
            buffer.ReadInt());
    }
}
#pragma warning restore CS1591

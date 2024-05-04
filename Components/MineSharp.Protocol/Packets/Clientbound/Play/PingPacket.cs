using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
/// Ping Packet https://wiki.vg/Protocol#Ping_.28play.29
/// </summary>
public class PingPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Play_Ping;

    /// <summary>
    /// The id of the ping
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Create a new instance
    /// </summary>
    /// <param name="id"></param>
    public PingPacket(int id)
    {
        this.Id = id;
    }

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteInt(this.Id);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new PingPacket(
            buffer.ReadInt());
    }
}

using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Status;
#pragma warning disable CS1591
public sealed partial record StatusRequestPacket : IPacketStatic<StatusRequestPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Status_PingStart;

    public void Write(PacketBuffer buffer, MinecraftData data)
    { }

    public static StatusRequestPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return new StatusRequestPacket();
    }
}
#pragma warning restore CS1591

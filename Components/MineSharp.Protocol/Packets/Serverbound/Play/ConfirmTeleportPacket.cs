using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public sealed partial record ConfirmTeleportPacket(int TeleportId) : IPacketStatic<ConfirmTeleportPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_TeleportConfirm;

    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(TeleportId);
    }

    public static ConfirmTeleportPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var teleportId = buffer.ReadVarInt();
        return new ConfirmTeleportPacket(teleportId);
    }
}
#pragma warning restore CS1591

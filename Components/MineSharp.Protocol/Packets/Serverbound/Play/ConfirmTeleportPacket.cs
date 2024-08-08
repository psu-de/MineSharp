using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public sealed record ConfirmTeleportPacket(int TeleportId) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_TeleportConfirm;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(TeleportId);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var teleportId = buffer.ReadVarInt();
        return new ConfirmTeleportPacket(teleportId);
    }
}
#pragma warning restore CS1591
